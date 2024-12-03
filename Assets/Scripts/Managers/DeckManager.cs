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
    private List<CardSO> allCards = new List<CardSO>();
    private List<CardSO> activeDeck = new List<CardSO>();
    private int currentDeckLimit;
    private int deckLimitMax;

    private void Awake()
    {
        if(Instance == null)
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
        Debug.Log($"Filtering cards of type: {effectType}");

        deckListContainer.Clear();

        foreach (CardSO card in allCards)
        {
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

    public bool TryAddCardToActiveDeck(CardSO card)
    {
        if (currentDeckLimit >= card.Cost)
        {
            activeDeck.Add(card);
            currentDeckLimit -= card.Cost;
            UpdateDeckLimitUI();
            AddMiniIcon(card);
            return true;
        }
        else
        {
            Debug.Log("Not enough space in the deck!");
            return false;
        }
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
        MiniIconUI iconUI = miniIcon.GetComponent<MiniIconUI>();
        iconUI.Configure(card.Icon, card.Cost);
    }

    private void RemoveMiniIcon(CardSO card)
    {
        foreach (Transform child in activeDeckParent)
        {
            MiniIconUI iconUI = child.GetComponent<MiniIconUI>();
            if (iconUI != null && iconUI.MatchesCard(card))
            {
                Destroy(child.gameObject);
                break;
            }
        }
    }
}
