using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardDraftManager : MonoBehaviour, IGameStateListener
{
    public static CardDraftManager Instance;

    [Header("UI References")]
    [SerializeField] private GameObject panel;
    [SerializeField] private Transform cardContainer;
    [SerializeField] private Button rerollButton;
    [SerializeField] private TextMeshProUGUI rerollCountText;
    [SerializeField] private Button skipButton;
    public CardEffectManager cardEffectManager { get; private set; }

    [Header("Card Sources")]
    [SerializeField] private CardLibrary cardLibrary;

    [Header("RARITY PREFABS")]
    [SerializeField] private CardOptionUI commonOptionPrefab;
    [SerializeField] private CardOptionUI uncommonOptionPrefab;
    [SerializeField] private CardOptionUI rareOptionPrefab;
    [SerializeField] private CardOptionUI epicOptionPrefab;
    [SerializeField] private CardOptionUI legendaryOptionPrefab;
    [SerializeField] private CardOptionUI mythicOptionPrefab;
    [SerializeField] private CardOptionUI exaltedOptionPrefab;

    private DraftType currentDraftType;
    private int rerollCount = 0;
    private int maxRerolls = 1;
    private int cardsNeeded;
    private int bonusDraftOptions = 0;
    private int rerollSuppression = 0;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        cardEffectManager = GetComponent<CardEffectManager>();
    }

    private void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.M))
        {
            ShowMajorDraft();
        }
        if (Input.GetKeyDown(KeyCode.N))
        {
            ShowMiniDraft();
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            rerollCount = 0;
        }
#endif
    }

    public void GameStateChangedCallback(GameState state)
    {
        if (state == GameState.CardDraft)
        {
            ShowDraft(DraftType.Major);
        }
        else
        {
            panel.SetActive(false);
        }
    }

    public void ShowMajorDraft()
    {
        ShowDraft(DraftType.Major);
    }

    public void ShowMiniDraft()
    {
        int chance = GetMiniDraftChance();
        int roll = Random.Range(0, 100);
        if (roll >= chance) return;

        ShowDraft(DraftType.Mini);
    }

    private int GetMiniDraftChance()
    {
        return 30;
    }

    private void ShowDraft(DraftType type)
    {
        currentDraftType = type;
        rerollCount = 0;

        panel.SetActive(true);
        RenderDraft();
    }

    private void RenderDraft()
    {
        List<CardSO> lockedCards = new List<CardSO>();

        // Remove non-locked slots and collect locked cards
        for (int i = cardContainer.childCount - 1; i >= 0; i--)
        {
            Transform child = cardContainer.GetChild(i);
            CardOptionUI slot = child.GetComponent<CardOptionUI>();

            if (slot != null && slot.IsLocked)
            {
                lockedCards.Add(slot.card);
            }
            else
            {
                Destroy(child.gameObject);
            }
        }

        bool isFallbackDraft = false;

        // 1️⃣ Get user-unlocked pool from CardLibrary
        List<CardSO> unlockedPool = cardLibrary.GetUnlockedUserCards();

        // 2️⃣ Check if we have enough
        if (unlockedPool.Count < 3)
        {
            isFallbackDraft = true;
            Debug.Log("[CardDraftManager] Not enough unlocked cards! Using fallback Free Use draft.");
        }

        // 3️⃣ Build pool to draw from
        List<CardSO> pool;
        if (isFallbackDraft)
        {
            // Use entire database
            pool = cardLibrary.allCards;
        }
        else
        {
            // Apply rarity + character filters
            List<string> characterPool = CharacterManager.Instance.CurrentCharacter.StartingCards
                .Select(card => card.cardID)
                .ToList();

            string currentCharacterID = CharacterManager.Instance.CurrentCharacter.ID;

            pool = cardLibrary.GetCardsByRarityAndID(
                CardDraftRarityConfig.Pools[currentDraftType],
                characterPool,
                currentCharacterID
            ).Where(card => UserManager.Instance.HasCardUnlocked(card.cardID)).ToList();
        }

        // 4️⃣ How many do we need?
        cardsNeeded = (currentDraftType == DraftType.Major ? 3 : 2) + bonusDraftOptions - lockedCards.Count;

        List<CardSO> randomCards = cardLibrary.PickRandomCards(pool, cardsNeeded);

        // 5️⃣ Add locked cards
        foreach (CardSO card in lockedCards)
        {
            CardOptionUI slot = Instantiate(GetOptionPrefab(card.rarity), cardContainer);
            slot.SetCard(card, () => OnCardSelected(card, false), true, false);
        }

        // 6️⃣ Add new draft cards
        foreach (CardSO card in randomCards)
        {
            CardOptionUI slot = Instantiate(GetOptionPrefab(card.rarity), cardContainer);
            slot.SetCard(card, () => OnCardSelected(card, isFallbackDraft), false, isFallbackDraft);
        }

        // 7️⃣ Reroll button setup
        if (currentDraftType == DraftType.Major && rerollButton != null)
        {
            int effectiveRerollLimit = maxRerolls - rerollSuppression;
            rerollCountText.text = maxRerolls.ToString();
            rerollButton.gameObject.SetActive(rerollCount < effectiveRerollLimit);
            rerollButton.onClick.RemoveAllListeners();
            rerollButton.onClick.AddListener(RerollDraft);
        }
    }

    public void RerollDraft()
    {
        if (rerollCount >= maxRerolls) return;
        rerollCount++;
        rerollCountText.text = (maxRerolls - rerollCount).ToString();
        AudioManager.Instance.PlayCrowdReaction(CrowdReactionType.Gasp);
        RenderDraft();
    }

    public void ModifyTacticalOverflow(int extraCards, int suppressRerolls)
    {
        bonusDraftOptions = extraCards;
        rerollSuppression = suppressRerolls;
    }

    private void OnCardSelected(CardSO card, bool wasFallback)
    {
        // Only unlock if it's not a Free Use fallback
        if (!wasFallback)
        {
            UserManager.Instance.UnlockCard(card.cardID);
        }

        CharacterCards cardSystem = CharacterManager.Instance.cards;

        if (cardSystem.currentTotalCost + card.cost <= cardSystem.GetEffectiveDeckCap())
        {
            cardSystem.AddCard(card);
            Debug.Log("Card selected: " + card.cardName);
        }
        else
        {
            Debug.Log("Card not added: deck cost would exceed limit.");
        }

        panel.SetActive(false);
    }

    public CardOptionUI GetOptionPrefab(CardRarity rarity)
    {
        switch (rarity)
        {
            case CardRarity.Common: return commonOptionPrefab;
            case CardRarity.Uncommon: return uncommonOptionPrefab;
            case CardRarity.Rare: return rareOptionPrefab;
            case CardRarity.Epic: return epicOptionPrefab;
            case CardRarity.Legendary: return legendaryOptionPrefab;
            case CardRarity.Mythic: return mythicOptionPrefab;
            case CardRarity.Exalted: return exaltedOptionPrefab;
            default: return commonOptionPrefab;
        }
    }

    public void SkipDraft()
    {
        AudioManager.Instance.PlayCrowdReaction(CrowdReactionType.Boo);
        panel.SetActive(false);
        GameManager.Instance.StartShop();
    }
}
