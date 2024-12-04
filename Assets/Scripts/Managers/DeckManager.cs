using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;   

public class DeckManager : MonoBehaviour
{
   public static DeckManager Instance;

    [Header("DECKLIST ELEMENTS:")]
    [SerializeField] private Transform deckListContainer;
    [SerializeField] private GameObject cardPrefab;

    [Header("CHARACTER ELEMENTS:")]
    [SerializeField] private Image characterIcon;
    [SerializeField] private TextMeshProUGUI characterNameText;
    [SerializeField] private Transform activeDeckParent;
    [SerializeField] private GameObject miniIconPrefab;
    [SerializeField] private TextMeshProUGUI deckLimitText;

    [Header("SETTINGS:")]
    [SerializeField] private string cardsResourceFolder = "Data/Cards";
    [SerializeField] private Canvas canvas;

    private List<CardSO> allCards = new List<CardSO>();
    private List<CardSO> activeDeck = new List<CardSO>();
    private int currentDeckLimit;
    private int deckLimitMax;
    private CardEffectType currentFilter;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        CharacterSelectionManager.OnCharacterSelected += UpdateDeckForCharacter;
    }

    private void OnDestroy()
    {
        CharacterSelectionManager.OnCharacterSelected -= UpdateDeckForCharacter;
    }

    private void Start()
    {
        deckLimitMax = CharacterManager.Instance.stats.CharacterData.DeckLimit;
        allCards.AddRange(Resources.LoadAll<CardSO>(cardsResourceFolder));
        FilterCards(CardEffectType.Damage);
        UpdateDeckLimitUI();
    }

    public void FilterCards(CardEffectType effectType)
    {
        currentFilter = effectType; 
        deckListContainer.Clear();

        foreach (CardSO card in allCards)
        {
            if (activeDeck.Contains(card))
                continue;

            if (card.EffectType == effectType && card.IsCollected)
            {
                GameObject newCard = Instantiate(cardPrefab, deckListContainer);
                CardsContainerUI cardUI = newCard.GetComponent<CardsContainerUI>();
                CardDragHandler dragHandler = newCard.GetComponent<CardDragHandler>();

                cardUI.Configure(card);
                dragHandler.Configure(card, this);
            }
        }
    }

    private void UpdateDeckForCharacter(CharacterDataSO characterData)
    {
        characterIcon.sprite = characterData.Icon;
        characterNameText.text = characterData.Name;
        currentDeckLimit = characterData.DeckLimit;
        deckLimitText.text = $"Deck Limit: {deckLimitMax}";
        FilterCards(CardEffectType.Damage);
    }

    public bool TryAddCardToActiveDeck(CardSO card, GameObject cardObject)
    {
        if (currentDeckLimit >= card.Cost)
        {
            activeDeck.Add(card);
            currentDeckLimit -= card.Cost;
            UpdateDeckLimitUI();
            AddMiniIcon(card);

            Destroy(cardObject); // Explicitly remove card from DeckList UI

            return true;
        }
        return false;
    }

    public void RemoveCardFromActiveDeck(CardSO card)
    {
        if (activeDeck.Contains(card))
        {
            activeDeck.Remove(card);
            currentDeckLimit += card.Cost;
            UpdateDeckLimitUI();
            RemoveMiniIcon(card);
        }
    }

    private void UpdateDeckLimitUI()
    {
        deckLimitText.text = $"Deck Limit: {deckLimitMax - currentDeckLimit}/{deckLimitMax}";
    }

    private void AddMiniIcon(CardSO card)
    {
        GameObject miniIcon = Instantiate(miniIconPrefab, activeDeckParent);
        MiniCardUI iconUI = miniIcon.GetComponent<MiniCardUI>();
        iconUI.Configure(card.Icon, card.Cost, card, this);
    }

    private void RemoveMiniIcon(CardSO card)
    {
        foreach (Transform child in activeDeckParent)
        {
            MiniCardUI iconUI = child.GetComponent<MiniCardUI>();
            if (iconUI != null && iconUI.MatchesCard(card))
            {
                Destroy(child.gameObject);
                break;
            }
        }
    }

    public void ReturnMiniCardToDeck(CardSO card, MiniCardUI miniCard)
    {
        if (activeDeck.Contains(card))
        {
            activeDeck.Remove(card);
            currentDeckLimit += card.Cost;
            UpdateDeckLimitUI();
        }

        Destroy(miniCard.gameObject);

        // Check if the card's EffectType matches the current filter
        if (currentFilter == card.EffectType)
        {
            GameObject newCard = Instantiate(cardPrefab, deckListContainer);
            CardsContainerUI cardUI = newCard.GetComponent<CardsContainerUI>();
            CardDragHandler dragHandler = newCard.GetComponent<CardDragHandler>();

            cardUI.Configure(card);
            dragHandler.Configure(card, this);
        }
        else
        {
            Debug.Log($"Card {card.CardName} does not match the current filter and was not re-added.");
        }
    }

    public float GetCanvasScaleFactor()
    {
        
        return canvas != null ? canvas.scaleFactor : 1f;
    }
}
