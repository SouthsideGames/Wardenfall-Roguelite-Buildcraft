using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Card Library Data", menuName = "Scriptable Objects/New Card Library Data", order = 8)]
public class CardLibrary : ScriptableObject
{
    public List<CardSO> allCards;

    public List<CardSO> GetUnlockedCards()
    {
        return allCards.FindAll(card => card.isUnlocked);
    }

    public CardSO GetCardByID(string id)
    {
        return allCards.Find(card => card.cardID == id);
    }
}
