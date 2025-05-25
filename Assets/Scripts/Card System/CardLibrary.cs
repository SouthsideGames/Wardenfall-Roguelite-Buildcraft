using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using SouthsideGames.SaveManager;

[CreateAssetMenu(fileName = "Card Library Data", menuName = "Scriptable Objects/New Card Library Data", order = 8)]
public class CardLibrary : ScriptableObject, IWantToBeSaved
{
    public static CardLibrary Instance;
    public List<CardSO> allCards = new List<CardSO>();

    private const string CardUnlockPrefix = "CardUnlock_";

    private void Awake()
    {
        Instance = this;
        LoadCardUnlockStates();
    }

    public List<CardSO> GetUnlockedCards()
    {
        return allCards.FindAll(card => card.isUnlocked);
    }

    public CardSO GetCardByID(string id)
    {
        return allCards.Find(card => card.cardID == id);
    }

    public List<CardSO> GetCardsByRarityAndID(CardRarity[] rarities, List<string> startingCards, string currentCharacterID)
    {
        return allCards.Where(card =>
            rarities.Contains(card.rarity) &&
            startingCards.Contains(card.cardID) &&
            card.isUnlocked
        ).ToList();
    }

    public List<CardSO> PickRandomCards(List<CardSO> pool, int count)
    {
        List<CardSO> result = new List<CardSO>();
        List<CardSO> tempPool = new List<CardSO>(pool);

        for (int i = 0; i < count && tempPool.Count > 0; i++)
        {
            int index = Random.Range(0, tempPool.Count);
            result.Add(tempPool[index]);
            tempPool.RemoveAt(index);
        }

        return result;
    }

    public void UnlockCardGlobally(string cardID)
    {
        CardSO card = GetCardByID(cardID);
        if (card != null && !card.isUnlocked)
        {
            card.isUnlocked = true;
            SaveManager.Save(this, CardUnlockPrefix + cardID, true);
        }
    }

    private void LoadCardUnlockStates()
    {
        foreach (CardSO card in allCards)
        {
            string key = CardUnlockPrefix + card.cardID;
            if (SaveManager.TryLoad(this, key, out object result) && result is bool unlocked && unlocked)
            {
                card.isUnlocked = true;
            }
        }
    }

    public void Save() {}
    public void Load() {}
}
