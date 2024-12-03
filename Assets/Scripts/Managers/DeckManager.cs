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

    /// <summary>
    /// Filters and displays cards in the deck list by type.
    /// </summary>
    public void FilterCards(CardEffectType effectType)
    {
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

    /// <summary>
    /// Updates the deck and character info based on the selected character.
    /// </summary>
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

        // Remove the card object from the DeckListContainer
        Destroy(cardObject);

        return true;
    }
    return false;
}

    /// <summary>
    /// Removes a card from the active deck.
    /// </summary>
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

    /// <summary>
    /// Updates the displayed deck limit.
    /// </summary>
    private void UpdateDeckLimitUI()
    {
        deckLimitText.text = $"Deck Limit: {deckLimitMax - currentDeckLimit}/{deckLimitMax}";
    }

    /// <summary>
    /// Adds a mini icon for the card to the active deck UI.
    /// </summary>
    private void AddMiniIcon(CardSO card)
    {
        GameObject miniIcon = Instantiate(miniIconPrefab, activeDeckParent);
        MiniCardUI iconUI = miniIcon.GetComponent<MiniCardUI>();
        iconUI.Configure(card.Icon, card.Cost, card, this);
    }

    /// <summary>
    /// Removes a mini icon for the card from the active deck UI.
    /// </summary>
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

    /// <summary>
    /// Returns a mini card to the deck list.
    /// </summary>
    public void ReturnMiniCardToDeck(CardSO card, MiniCardUI miniCard)
    {
        if (activeDeck.Contains(card))
        {
            activeDeck.Remove(card);
            currentDeckLimit += card.Cost;
            UpdateDeckLimitUI();
        }

        Destroy(miniCard.gameObject);

        GameObject newCard = Instantiate(cardPrefab, deckListContainer);
        CardsContainerUI cardUI = newCard.GetComponent<CardsContainerUI>();
        CardDragHandler dragHandler = newCard.GetComponent<CardDragHandler>();

        cardUI.Configure(card);
        dragHandler.Configure(card, this);
    }

    /// <summary>
    /// Retrieves the canvas scale factor.
    /// </summary>
    public float GetCanvasScaleFactor()
    {
        
        return canvas != null ? canvas.scaleFactor : 1f;
    }
}
