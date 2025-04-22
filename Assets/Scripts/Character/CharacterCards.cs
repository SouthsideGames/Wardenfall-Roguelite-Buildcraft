using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CharacterCards : MonoBehaviour
{
    public static event Action OnDeckChanged;

    private const int baseDeckCost = 10;
    private int cardCapModifier = 0;
    private List<CardSO> currentDeck = new List<CardSO>();
    public int currentTotalCost;

    public IReadOnlyList<CardSO> Deck => currentDeck;

    public void AddCard(CardSO newCard)
    {
        if (currentTotalCost + newCard.cost <= GetEffectiveDeckCap())
        {
            currentDeck.Add(newCard);
            currentTotalCost += newCard.cost;
            OnDeckChanged?.Invoke();
        }
        else
        {
            CardDraftUI.Instance.ShowReplaceCardPrompt(newCard, currentDeck, currentTotalCost, baseDeckCost);
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

    public void RemoveCard(CardSO cardToRemove)
    {
        if (currentDeck.Contains(cardToRemove))
        {
            currentDeck.Remove(cardToRemove);
            currentTotalCost -= cardToRemove.cost;
            OnDeckChanged?.Invoke();
        }
    }


#region For Traits

    public void ModifyCardCap(int change) => cardCapModifier += change;
    public int GetEffectiveDeckCap() => baseDeckCost + cardCapModifier;

#endregion

} 
