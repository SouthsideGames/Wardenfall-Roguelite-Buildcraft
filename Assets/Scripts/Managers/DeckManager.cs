using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using SouthsideGames.SaveManager;
using System;

public class DeckManager : MonoBehaviour, IWantToBeSaved
{
    public static DeckManager Instance;

    [Header("DECKLIST ELEMENTS:")]
    [SerializeField] private Transform deckListContainer;
    [SerializeField] private GameObject cardDetailContainer;
    public CardDetailUI cardDetailUI { get; private set; }


    [Header("CHARACTER ELEMENTS:")]
    [SerializeField] private Image characterIcon;
    [SerializeField] private TextMeshProUGUI characterNameText;
    [SerializeField] private Transform activeDeckParent;
    [SerializeField] private TextMeshProUGUI deckLimitText;

    [Header("CARD FRAMES BY RARITY")]
    [SerializeField] private List<CardFrameMapping> cardFramesByRarity;
    [SerializeField] private List<MiniCardFrameMapping> miniCardFramesByRarity;


    [Header("SETTINGS:")]
    [SerializeField] private string cardsResourceFolder = "Data/Cards";
    [SerializeField] private Canvas canvas;

    private List<CardSO> allCards = new List<CardSO>();
    private List<CardSO> activeDeck = new List<CardSO>();
    private List<string> savedCardIDs = new List<string>();
    private Dictionary<CardRarityType, GameObject> cardFrameDictionary;
    private Dictionary<MiniCardRarityType, GameObject> miniCardFrameDictionary;
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
        CardDragHandlerUI.OnButtonPressed += ShowCardDetails;

        cardDetailUI = cardDetailContainer.GetComponent<CardDetailUI>();
        InitializeCardFrames();
        InitializeMiniCardFrames();

    }

    private void OnDestroy()
    {
        CharacterSelectionManager.OnCharacterSelected -= UpdateDeckForCharacter;
        CardDragHandlerUI.OnButtonPressed -= ShowCardDetails;
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

    private void InitializeMiniCardFrames()
    {
        miniCardFrameDictionary = new Dictionary<MiniCardRarityType, GameObject>();
        foreach (MiniCardFrameMapping mapping in miniCardFramesByRarity)
        {
            if (!miniCardFrameDictionary.ContainsKey(mapping.rarity))
            {
                miniCardFrameDictionary.Add(mapping.rarity, mapping.miniCardFramePrefab);
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
        DecklistCardUI cardUI = newCard.GetComponent<DecklistCardUI>();
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
                DecklistCardUI cardUI = newCard.GetComponent<DecklistCardUI>();
                CardDragHandlerUI dragHandler = newCard.GetComponent<CardDragHandlerUI>();

                cardUI.Configure(card);
                dragHandler.Configure(card, this);
            }
        }
    }

    private void AddMiniIcon(CardSO card)
    {
       if (!miniCardFrameDictionary.TryGetValue((MiniCardRarityType)card.Rarity, out GameObject miniFramePrefab))
            return;

        GameObject miniIcon = Instantiate(miniFramePrefab, activeDeckParent);
        MiniDecklistCardUI iconUI = miniIcon.GetComponent<MiniDecklistCardUI>();
        iconUI.Configure(card.Icon, card.Cost, card, this);

        UpdateDeckLimitUI();
    }

    private void RemoveMiniIcon(CardSO card)
    {
        foreach (Transform child in activeDeckParent)
        {
            MiniDecklistCardUI iconUI = child.GetComponent<MiniDecklistCardUI>();
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

    public void ReturnMiniCardToDeck(CardSO card, MiniDecklistCardUI miniCard)
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
            DecklistCardUI cardUI = newCard.GetComponent<DecklistCardUI>();
            CardDragHandlerUI dragHandler = newCard.GetComponent<CardDragHandlerUI>();

            cardUI.Configure(card);
            dragHandler.Configure(card, this);
        }
    }

    public void ShowCardDetails(CardSO card)
    {
        cardDetailContainer.SetActive(true);
        if (cardDetailUI != null)
            cardDetailUI.ShowDetail(card);
    }

    public void CloseCardDetails() => cardDetailContainer.SetActive(false);

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
    public MiniCardRarityType rarity;
    public GameObject miniCardFramePrefab;
}

