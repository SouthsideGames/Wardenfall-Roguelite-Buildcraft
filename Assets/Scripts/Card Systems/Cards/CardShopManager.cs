using System;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using SouthsideGames.SaveManager;
using UnityEngine;

public class CardShopManager : MonoBehaviour, IWantToBeSaved
{
    public static event Action OnWeeklyRefresh;

    [Header("ELEMENTS")]
    [SerializeField] private Transform shopContainer; 
    [SerializeField] private List<ShopCardFrameMapping> cardFramesByRarity; 
    [SerializeField] private string cardsResourceFolder = "Data/Cards"; 

    private const string LastRefreshKey = "LastShopRefresh";
    private const string WeeklyShopCardsKey = "WeeklyShopCards";

    private Dictionary<CardRarityType, GameObject> shopCardFrameDictionary; 
    private List<CardSO> allCards; 
    private List<CardSO> currentShopCards;

    private void Start()
    {
        InitializeCardFrames();
        LoadAllCards();
        Load(); 
        CheckWeeklyRefresh();
        PopulateShop();
    }

    private void InitializeCardFrames()
    {
        shopCardFrameDictionary = new Dictionary<CardRarityType, GameObject>();
        foreach (var mapping in cardFramesByRarity)
        {
            if (!shopCardFrameDictionary.ContainsKey(mapping.rarity))
            {
                shopCardFrameDictionary.Add(mapping.rarity, mapping.shopCardFramePrefab);
            }
        }
    }

    private void LoadAllCards()
    {
        allCards = Resources.LoadAll<CardSO>(cardsResourceFolder).ToList();
        if (allCards.Count == 0)
        {
            Debug.LogError("No cards found in the specified Resources folder!");
        }
    }

    private void CheckWeeklyRefresh()
    {
        if (SaveManager.TryLoad(this, LastRefreshKey, out object savedTime) && DateTime.TryParse(savedTime.ToString(), out DateTime lastRefresh))
        {
            if ((DateTime.Now - lastRefresh).Days >= 7)
            {
                SelectWeeklyCards();
                Save();
            
            }
            else
            {
                LoadWeeklyShop();
            }
        }
        else
        {
            SelectWeeklyCards();
            Save();
        }
    }

    private void SelectWeeklyCards()
    {
        OnWeeklyRefresh?.Invoke();
        currentShopCards = new List<CardSO>();
        for (int i = 0; i < 5; i++)
        {
            CardSO randomCard = allCards[UnityEngine.Random.Range(0, allCards.Count)];
            currentShopCards.Add(randomCard);
        }
    }

    private void PopulateShop()
    {
        foreach (Transform child in shopContainer)
        {
            Destroy(child.gameObject);
        }

        foreach (var card in currentShopCards)
        {
            if (!shopCardFrameDictionary.TryGetValue(card.Rarity, out GameObject framePrefab))
            {
                Debug.LogWarning($"No frame found for card rarity {card.Rarity}.");
                continue;
            }

            GameObject shopCard = Instantiate(framePrefab, shopContainer);
            CardShopUI shopCardUI = shopCard.GetComponent<CardShopUI>();
            shopCardUI.Configure(card);
        }
    }

    private void LoadWeeklyShop()
    {
        if (SaveManager.TryLoad(this, WeeklyShopCardsKey, out object savedCards) && savedCards is List<string> cardIDs)
        {
            currentShopCards = allCards.Where(card => cardIDs.Contains(card.ID)).ToList();
        }
        else
        {
            currentShopCards = new List<CardSO>();
            Debug.LogWarning("No saved shop data found; creating a new shop selection.");
            SelectWeeklyCards();
        }
    }

    public void Save()
    {
        SaveManager.Save(this, LastRefreshKey, DateTime.Now.ToString());
        List<string> cardIDs = currentShopCards.Select(card => card.ID).ToList();
        SaveManager.Save(this, WeeklyShopCardsKey, cardIDs);
    }

    public void Load()
    {
        if (allCards == null || allCards.Count == 0)
            allCards = new List<CardSO>();

        if (SaveManager.TryLoad(this, WeeklyShopCardsKey, out object savedCards) && savedCards is List<string> cardIDs)
        {
            currentShopCards = allCards.Where(card => cardIDs.Contains(card.ID)).ToList();
        }
        else
        {
            currentShopCards = new List<CardSO>();
        }
    }


    [Button("Refresh Shop")]
    public void TestRefreshShop()
    {
        ForceRefreshShop();
    }

    public void ForceRefreshShop()
    {
        SelectWeeklyCards();
        PopulateShop();
        Save();
        Debug.Log("Shop manually refreshed with new cards.");
    }
}

[Serializable]
public class ShopCardFrameMapping
{
    public CardRarityType rarity;
    public GameObject shopCardFramePrefab;
}