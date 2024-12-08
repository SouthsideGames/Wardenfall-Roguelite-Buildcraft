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
    

    [Header("CHARACTER ELEMENTS:")]
    [SerializeField] private Image characterIcon;
    [SerializeField] private TextMeshProUGUI characterNameText;
    [SerializeField] private Transform activeDeckParent;
    [SerializeField] private GameObject miniIconPrefab;
    [SerializeField] private TextMeshProUGUI deckLimitText;

    [Header("CARD FRAMES BY RARITY")]
    [SerializeField] private List<CardFrameMapping> cardFramesByRarity;

    [Header("SETTINGS:")]
    [SerializeField] private string cardsResourceFolder = "Data/Cards";
    [SerializeField] private Canvas canvas;

    private List<CardSO> allCards = new List<CardSO>();
    private List<CardSO> activeDeck = new List<CardSO>();
    private List<string> savedCardIDs = new List<string>();
    private Dictionary<CardRarityType, GameObject> cardFrameDictionary;
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

        InitializeCardFrames();
    }

    private void OnDestroy()
    {
        CharacterSelectionManager.OnCharacterSelected -= UpdateDeckForCharacter;
    }

    private void Start()
    {
        allCards.AddRange(Resources.LoadAll<CardSO>(cardsResourceFolder));

        Load();

    
        UpdateActiveDeckFromSavedIDs();
        UpdateDeckLimitUI();
        FilterCards(CardEffectType.None);
    }

    private void OnDisable()
    {
        Save();
    }

    public void Save()
    {
        savedCardIDs = activeDeck.Select(card => card.ID).ToList();

        SaveManager.Save(this, "ActiveDeckCardIDs", savedCardIDs);
        Debug.Log($"Saved Active Deck IDs: {string.Join(", ", savedCardIDs)}");
    }

    public void Load()
    {
        if (SaveManager.TryLoad(this, "ActiveDeckCardIDs", out object loadedData) && loadedData is List<string> loadedIDs)
        {
            savedCardIDs = loadedIDs;
        }

    }

    private void InitializeCardFrames()
    {
        cardFrameDictionary = new Dictionary<CardRarityType, GameObject>();
        foreach (CardFrameMapping mapping in cardFramesByRarity)
        {
            if (!cardFrameDictionary.ContainsKey(mapping.rarity))
            {
                cardFrameDictionary.Add(mapping.rarity, mapping.cardFramePrefab);
            }
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
        }
        

        CharacterManager.Instance.deck.FillEquippedCardsFromSavedIDs(allCards, savedCardIDs);
        UpdateDeckLimitUI();
    }

    public bool TryAddCardToActiveDeck(CardSO card, GameObject cardObject)
    {
        int newDeckCost = activeDeck.Sum(c => c.Cost) + card.Cost;

        if (newDeckCost <= deckLimitMax)
        {
            activeDeck.Add(card);           
            CharacterManager.Instance.deck.AddCard(card); 
            UpdateDeckLimitUI();  
            AddMiniIcon(card);               
            Destroy(cardObject);            
            Save();   

            return true;
        }
        else
            return false;
    }

    public void RemoveCardFromActiveDeck(CardSO card)
    {
        if (activeDeck.Contains(card))
        {
            activeDeck.Remove(card);         
            CharacterManager.Instance.deck.RemoveCard(card);
            UpdateDeckLimitUI();            
            RemoveMiniIcon(card);            
            Save();                         
        }

        FilterCards(currentFilter);
    }

    public GameObject CreateCard(CardSO card, Transform parent)
    {
        if (!cardFrameDictionary.TryGetValue(card.Rarity, out GameObject framePrefab))
            return null;

        GameObject newCard = Instantiate(framePrefab, parent);
        DecklistCardContainerUI cardUI = newCard.GetComponent<DecklistCardContainerUI>();
        cardUI.Configure(card);
        return newCard;
    }

    public void FilterCards(CardEffectType effectType)
    {
        currentFilter = effectType;

        foreach (Transform child in deckListContainer)
            Destroy(child.gameObject);

        foreach (CardSO card in allCards)
        {
            if (!activeDeck.Contains(card) && 
                (effectType == CardEffectType.None || card.EffectType == effectType))
            {
                if (!cardFrameDictionary.TryGetValue(card.Rarity, out GameObject framePrefab))
                    continue;

                GameObject newCard = Instantiate(framePrefab, deckListContainer);
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

        UpdateDeckLimitUI();
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
        int currentDeckCost = activeDeck.Sum(card => card.Cost); // Correct total cost
        deckLimitText.text = $"Deck Limit: {currentDeckCost}/{deckLimitMax}";
    }

    private void UpdateDeckForCharacter(CharacterDataSO _characterData)
    {
        characterIcon.sprite = _characterData.Icon;
        characterNameText.text = _characterData.Name;
        deckLimitMax = _characterData.DeckLimit;
        UpdateDeckLimitUI();   
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
            CharacterManager.Instance.deck.RemoveCard(card);
            currentDeckLimit += card.Cost;
            UpdateDeckLimitUI();
        }

        Destroy(miniCard.gameObject);

        if (currentFilter == card.EffectType && 
            cardFrameDictionary.TryGetValue(card.Rarity, out GameObject framePrefab))
        {
            GameObject newCard = Instantiate(framePrefab, deckListContainer);
            DecklistCardContainerUI cardUI = newCard.GetComponent<DecklistCardContainerUI>();
            CardDragHandlerUI dragHandler = newCard.GetComponent<CardDragHandlerUI>();

            cardUI.Configure(card);
            dragHandler.Configure(card, this);
        }
    }

}

[System.Serializable]
public class CardFrameMapping
{
    public CardRarityType rarity;
    public GameObject cardFramePrefab;
}

[System.Serializable]
public class MiniCardFrameMapping
{
    public CardRarityType rarity;
    public GameObject miniCardFramePrefab;
}

