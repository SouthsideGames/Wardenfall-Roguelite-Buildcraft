using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "Card Library Data", menuName = "Scriptable Objects/New Card Library Data", order = 8)]
public class CardLibrary : ScriptableObject
{
    public static CardLibrary Instance;
    public List<CardSO> allCards = new List<CardSO>();

    private void Awake() => Instance = this;

    public List<CardSO> GetUnlockedCards()
    {
        return allCards.FindAll(card => card.isUnlocked);
    }

    public CardSO GetCardByID(string id)
    {
        return allCards.Find(card => card.cardID == id);
    }

    public List<CardSO> GetCardsByRarity(CardRarity[] allowedRarities)
    {
        return allCards.Where(c => allowedRarities.Contains(c.rarity) && c.isUnlocked).ToList();
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
}