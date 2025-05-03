using System.Collections.Generic;
using UnityEngine;

public class CardDraftManager : MonoBehaviour, IGameStateListener
{
    public static CardDraftManager Instance;

    [SerializeField] private GameObject panel;
    [SerializeField] private Transform cardContainer;
    [SerializeField] private CardLibrary cardLibrary;

    [Header("RARITY PREFABS")]
    [SerializeField] private CardOptionUI commonOptionPrefab;
    [SerializeField] private CardOptionUI uncommonOptionPrefab;
    [SerializeField] private CardOptionUI rareOptionPrefab;
    [SerializeField] private CardOptionUI epicOptionPrefab;
    [SerializeField] private CardOptionUI legendaryOptionPrefab;
    [SerializeField] private CardOptionUI mythicOptionPrefab;
    [SerializeField] private CardOptionUI exaltedOptionPrefab;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void GameStateChangedCallback(GameState state)
    {
        if (state == GameState.CardDraft)
            OpenCardDraft();
        else
            CloseCardDraft();
    }

    private void OpenCardDraft()
    {
        ShowDraft(DraftType.Major);
    }

    public void ShowMajorDraft()
    {
        ShowDraft(DraftType.Major);
    }

    public void ShowMiniDraft()
    {
        if (Random.Range(0, 100) >= 30) return;
        ShowDraft(DraftType.Mini);
    }

    private void ShowDraft(DraftType type)
    {
        panel.SetActive(true);

        foreach (Transform child in cardContainer)
            Destroy(child.gameObject);

        var pool = cardLibrary.GetCardsByRarity(CardDraftRarityConfig.Pools[type]);
        var cardsToShow = cardLibrary.PickRandomCards(pool, type == DraftType.Major ? 3 : 2);

        foreach (CardSO cardSO in cardsToShow)
        {
            CardOptionUI cardUI = Instantiate(GetOptionPrefab(cardSO.rarity), cardContainer);
            cardUI.SetCard(cardSO, () => OnCardSelected(cardSO));
        }
    }

    public CardOptionUI GetOptionPrefab(CardRarity rarity)
    {
        return rarity switch
        {
            CardRarity.Common => commonOptionPrefab,
            CardRarity.Uncommon => uncommonOptionPrefab,
            CardRarity.Rare => rareOptionPrefab,
            CardRarity.Epic => epicOptionPrefab,
            CardRarity.Legendary => legendaryOptionPrefab,
            CardRarity.Mythic => mythicOptionPrefab,
            CardRarity.Exalted => exaltedOptionPrefab,
            _ => commonOptionPrefab
        };
    }

    private void CloseCardDraft()
    {
        panel.SetActive(false);
    }

    private void OnCardSelected(CardSO card)
    {
        CharacterManager.Instance.cards.AddCard(card); 
        CloseCardDraft();
    }

} 
