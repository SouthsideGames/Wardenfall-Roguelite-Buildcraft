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
        CardUnlockManager.Instance?.LoadAllCardUnlockStates(this);
    }

    public List<CardSO> GetUnlockedCards()
    {
        return allCards.FindAll(card => card.unlockData.unlocked);
    }

    public CardSO GetCardByID(string id)
    {
        return allCards.Find(card => card.cardID == id);
    }

    public List<CardSO> GetCardsByRarityAndID(CardRarity[] rarities, List<string> excludedCardIDs, string currentCharacterID)
    {
        var temporaryUnlocks = CharacterManager.Instance?.cards.TemporaryUnlockedCards ?? new List<CardSO>();

        return allCards.Where(card =>
            rarities.Contains(card.rarity) &&
            !excludedCardIDs.Contains(card.cardID) &&
            (card.unlockData.unlocked || temporaryUnlocks.Contains(card))
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
        if (card != null && !card.unlockData.unlocked)
        {
            card.unlockData.unlocked = true;
            SaveManager.Save(this, CardUnlockPrefix + cardID, true);
        }
    }

    public void Save() { }
    public void Load() { }
}
