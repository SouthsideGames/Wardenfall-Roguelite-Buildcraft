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
    public int currentTotalCost { get; private set; }
    public IReadOnlyList<CardSO> Deck => currentDeck;
    public List<CardSO> TemporaryUnlockedCards { get; private set; } = new List<CardSO>();

    public void AddCard(CardSO newCard)
    {
        if (newCard == null) return;

        currentDeck.Add(newCard);
        currentTotalCost += newCard.cost;
        OnDeckChanged?.Invoke();

        // Apply synergies if manager exists
        CardSynergyManager.Instance?.CheckAndApplySynergies(this);
    }

    public void RemoveCard(CardSO cardToRemove)
    {
        if (!currentDeck.Contains(cardToRemove)) return;

        currentDeck.Remove(cardToRemove);
        currentTotalCost -= cardToRemove.cost;
        OnDeckChanged?.Invoke();
    }

    public void SetCurrentCharacterStartingCards(CharacterDataSO characterData)
    {
        TemporaryUnlockedCards = new List<CardSO>(characterData.StartingCards);
    }

    #region For Traits

    public void ModifyCardCap(int change) => cardCapModifier += change;
    public int GetEffectiveDeckCap() => baseDeckCost + cardCapModifier;
    public bool HasCard(string cardID) => currentDeck.Any(card => card.cardID == cardID);

    #endregion
}
