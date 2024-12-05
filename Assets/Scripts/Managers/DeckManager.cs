using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using SouthsideGames.SaveManager;

public class DeckManager : MonoBehaviour, IWantToBeSaved
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
    private List<string> savedCardIDs = new List<string>();
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
        // Load all available cards
        allCards.AddRange(Resources.LoadAll<CardSO>(cardsResourceFolder));

        // Load saved card IDs
        Load();

        // Update the active deck with saved data
        UpdateActiveDeckFromSavedIDs();

        // Set initial deck limit
        deckLimitMax = CharacterManager.Instance.stats.CharacterData.DeckLimit;
        UpdateDeckLimitUI();
    }

    private void OnDisable()
    {
        Save();
    }

    public void Save()
    {
        // Clear and update the savedCardIDs list
        savedCardIDs = activeDeck.Select(card => card.ID).ToList();

        // Save the list to the SaveManager
        SaveManager.Save(this, "ActiveDeckCardIDs", savedCardIDs);
        Debug.Log($"Saved Active Deck IDs: {string.Join(", ", savedCardIDs)}");
    }

    public void Load()
    {
        // Load saved card IDs
        if (SaveManager.TryLoad(this, "ActiveDeckCardIDs", out object loadedData) && loadedData is List<string> loadedIDs)
        {
            savedCardIDs = loadedIDs;
            Debug.Log($"Loaded Active Deck IDs: {string.Join(", ", savedCardIDs)}");
        }
        else
        {
            Debug.LogWarning("No saved data found for Active Deck.");
        }
    }

    public void UpdateActiveDeckFromSavedIDs()
    {
        activeDeck.Clear();
        CharacterManager.Instance.deck.ClearDeck();

        foreach (string cardID in savedCardIDs)
        {
            CardSO matchingCard = allCards.FirstOrDefault(card => card.ID == cardID);
            if (matchingCard != null)
            {
                activeDeck.Add(matchingCard);
                CharacterManager.Instance.deck.AddCard(matchingCard);
                AddMiniIcon(matchingCard);
            }
            else
            {
                Debug.LogWarning($"Card with ID {cardID} not found in resources.");
            }
        }
        

         CharacterManager.Instance.deck.FillEquippedCardsFromSavedIDs(allCards, savedCardIDs);
        UpdateDeckLimitUI();
    }

    public bool TryAddCardToActiveDeck(CardSO card, GameObject cardObject)
    {
        if (CharacterManager.Instance.deck.AddCard(card))
        {
            activeDeck.Add(card);
            currentDeckLimit -= card.Cost;
            UpdateDeckLimitUI();
            AddMiniIcon(card);

            Destroy(cardObject);
            Save();

            return true;
        }
        else
        {
            Debug.Log($"Card {card.CardName} exceeds the character's deck limit.");
            return false;
        }
    }

    public void RemoveCardFromActiveDeck(CardSO card)
    {
        if (activeDeck.Contains(card))
        {
            activeDeck.Remove(card);
            CharacterManager.Instance.deck.RemoveCard(card);
            currentDeckLimit += card.Cost;
            UpdateDeckLimitUI();
            RemoveMiniIcon(card);
            Save();
        }
    }

    public void FilterCards(CardEffectType effectType)
    {
        currentFilter = effectType;
        deckListContainer.Clear();

        foreach (CardSO card in allCards)
        {
            if (!activeDeck.Contains(card) && (effectType == default || card.EffectType == effectType))
            {
                GameObject newCard = Instantiate(cardPrefab, deckListContainer);
                DecklistCardContainerUI cardUI = newCard.GetComponent<DecklistCardContainerUI>();
                CardDragHandlerUI dragHandler = newCard.GetComponent<CardDragHandlerUI>();

                cardUI.Configure(card);
                dragHandler.Configure(card, this);
            }
        }
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

    private void UpdateDeckLimitUI()
    {
        deckLimitText.text = $"Deck Limit: {activeDeck.Count}/{deckLimitMax}";
    }

    private void UpdateDeckForCharacter(CharacterDataSO characterData)
    {
        characterIcon.sprite = characterData.Icon;
        characterNameText.text = characterData.Name;
        currentDeckLimit = characterData.DeckLimit;
        deckLimitText.text = $"Deck Limit: {deckLimitMax}";
        FilterCards(CardEffectType.Damage);
    }

    public float GetCanvasScaleFactor()
    {
        return canvas != null ? canvas.scaleFactor : 1f;
    }

       public void ReturnMiniCardToDeck(CardSO card, MiniCardUI miniCard)
    {
        if (activeDeck.Contains(card))
        {
            activeDeck.Remove(card);

            // Update the CharacterDeck
            CharacterManager.Instance.deck.RemoveCard(card);

            currentDeckLimit += card.Cost;
            UpdateDeckLimitUI();
        }

        Destroy(miniCard.gameObject);

        // Check if the card's EffectType matches the current filter
        if (currentFilter == card.EffectType)
        {
            GameObject newCard = Instantiate(cardPrefab, deckListContainer);
            DecklistCardContainerUI cardUI = newCard.GetComponent<DecklistCardContainerUI>();
            CardDragHandlerUI dragHandler = newCard.GetComponent<CardDragHandlerUI>();

            cardUI.Configure(card);
            dragHandler.Configure(card, this);
        }
        else
        {
            Debug.Log($"Card {card.CardName} does not match the current filter and was not re-added.");
        }
    }



}
