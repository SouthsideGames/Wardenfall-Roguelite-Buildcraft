
using System.Collections.Generic;
using UnityEngine;

public class SynergyManager : MonoBehaviour
{
  public List<Synergy> synergies = new List<Synergy>();

    public void CheckAndApplySynergies(CharacterCards deck)
    {
        foreach (var synergy in synergies)
        {
            bool hasAllCards = synergy.requiredCards.All(cardID => deck.HasCard(cardID));
            if (hasAllCards)
            {
                foreach (var requiredCard in synergy.requiredCards)
                {
                    deck.RemoveCard(deck.Deck.First(x=>x.cardID == requiredCard));
                }
                deck.AddCard(synergy.resultingCard);
                break; // Apply only the first matching synergy
            }
        }
    }
}
