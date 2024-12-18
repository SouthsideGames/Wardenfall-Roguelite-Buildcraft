using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterDeck : MonoBehaviour
{
    [Header("ELEMENTS:")]
    [SerializeField] private List<CardSO> equippedCards = new List<CardSO>(); 

    public int deckLimit = 10;
    private CharacterDataSO characterData;

    private void Start()
    {
        characterData = CharacterManager.Instance.stats.CharacterData;

    }

    public bool AddCard(CardSO card)
    {
        if (GetCurrentDeckCost() + card.Cost <= deckLimit)
        {
            equippedCards.Add(card);
            Debug.Log($"Added {card.CardName} to the deck.");
            return true;
        }
        else
        {
            Debug.Log($"Cannot add {card.CardName}. Exceeds deck limit.");
            return false;
        }
    }

    public void RemoveCard(CardSO card)
    {
        if (equippedCards.Contains(card))
        {
            equippedCards.Remove(card);
            Debug.Log($"Removed {card.CardName} from the deck.");
        }
        else
        {
            Debug.Log($"Card {card.CardName} is not in the deck.");
        }
    }

    public void ClearDeck()
    {
        equippedCards.Clear();
        Debug.Log("Deck cleared.");
    }

    public int GetCurrentDeckCost()
    {
        int totalCost = 0;
        foreach (var card in equippedCards)
        {
            totalCost += card.Cost;
        }
        return totalCost;
    }


    public List<CardSO> GetEquippedCards()
    {
        return new List<CardSO>(equippedCards);
    }

    public bool CanAddCard(CardSO card)
    {
        return GetCurrentDeckCost() + card.Cost <= deckLimit;
    }

    public void PrintDeck()
    {
        Debug.Log($"Deck for {characterData.Name} (Limit: {deckLimit}):");
        foreach (var card in equippedCards)
        {
            Debug.Log($"- {card.CardName} (Cost: {card.Cost})");
        }
    }

    public void FillEquippedCardsFromSavedIDs(List<CardSO> allCards, List<string> savedCardIDs)
    {
        equippedCards.Clear();

        foreach (string cardID in savedCardIDs)
        {
            CardSO matchingCard = allCards.Find(card => card.ID == cardID);
            if (matchingCard != null)
            {
                equippedCards.Add(matchingCard);
                Debug.Log($"Added {matchingCard.CardName} to equipped cards.");
            }
            else
            {
                Debug.LogWarning($"Card with ID {cardID} not found.");
            }
        }
    }
}
