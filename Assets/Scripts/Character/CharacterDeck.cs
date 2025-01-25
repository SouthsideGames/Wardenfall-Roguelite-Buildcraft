using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterDeck : MonoBehaviour
{
    [Header("ELEMENTS:")]
    [SerializeField] private List<CardSO> equippedCards = new List<CardSO>(); 

    public int deckLimit = 10;
    private CharacterDataSO characterData;

    private void Start() => characterData = CharacterManager.Instance.stats.CharacterData;

    public bool AddCard(CardSO card)
    {
        if (GetCurrentDeckCost() + card.Cost <= deckLimit)
        {
            equippedCards.Add(card);
            return true;
        }
        else
            return false;
    }

    public void RemoveCard(CardSO card)
    {
        if (equippedCards.Contains(card))
            equippedCards.Remove(card);
    }

    public void ClearDeck() => equippedCards.Clear();
    public int GetCurrentDeckCost()
    {
        int totalCost = 0;
        foreach (var card in equippedCards)
        {
            totalCost += card.Cost;
        }
        return totalCost;
    }

    public List<CardSO> GetEquippedCards() => new List<CardSO>(equippedCards);

    public bool CanAddCard(CardSO card) => GetCurrentDeckCost() + card.Cost <= deckLimit;

    public void FillEquippedCardsFromSavedIDs(List<CardSO> allCards, List<string> savedCardIDs)
    {
        equippedCards.Clear();

        foreach (string cardID in savedCardIDs)
        {
            CardSO matchingCard = allCards.Find(card => card.ID == cardID);
            if (matchingCard != null)
                equippedCards.Add(matchingCard);
      
        }
    }
}
