using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CharacterCards : MonoBehaviour
{
    public static event Action OnDeckChanged;

    private const int maxDeckCost = 10;

    private List<CardSO> currentDeck = new List<CardSO>();
    private int currentTotalCost;

    public IReadOnlyList<CardSO> Deck => currentDeck;

    public void AddCard(CardSO newCard)
    {
        if (currentTotalCost + newCard.cost <= maxDeckCost)
        {
            currentDeck.Add(newCard);
            currentTotalCost += newCard.cost;
            OnDeckChanged?.Invoke();
        }
        else
        {
            CardDraftUI.Instance.ShowReplaceCardPrompt(newCard, currentDeck, currentTotalCost, maxDeckCost);
        }
    }

    public void ReplaceCard(int index, CardSO newCard)
    {
        currentTotalCost -= currentDeck[index].cost;
        currentTotalCost += newCard.cost;
        currentDeck[index] = newCard;
        OnDeckChanged?.Invoke();
    }

    public void SkipCard()
    {
        CardDraftUI.Instance.HideReplaceCardPrompt();
        OnDeckChanged?.Invoke();
    }

    public void ClearDeck()
    {
        currentDeck.Clear();
        currentTotalCost = 0;
        OnDeckChanged?.Invoke();
    }
} 
