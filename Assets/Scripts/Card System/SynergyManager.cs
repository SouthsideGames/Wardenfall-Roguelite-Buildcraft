
using System.Collections.Generic;
using UnityEngine;

public class SynergyManager : MonoBehaviour
{
    [SerializeField] private List<CardSynergy> availableSynergies = new();
    
    public void CheckAndApplySynergies(CharacterCards characterCards)
    {
        var deck = characterCards.Deck;
        
        foreach (var synergy in availableSynergies)
        {
            CardSO cardA = null;
            CardSO cardB = null;
            
            // Find matching cards in deck
            foreach (var card in deck)
            {
                if (card.effectType == synergy.cardTypeA && cardA == null)
                    cardA = card;
                else if (card.effectType == synergy.cardTypeB && cardB == null)
                    cardB = card;
                
                if (cardA != null && cardB != null)
                    break;
            }
            
            // If we found both cards, apply synergy
            if (cardA != null && cardB != null)
            {
                int indexA = deck.IndexOf(cardA);
                int indexB = deck.IndexOf(cardB);
                
                // Remove both cards and add synergy card
                characterCards.RemoveCard(indexA);
                characterCards.RemoveCard(indexB > indexA ? indexB - 1 : indexB);
                characterCards.AddCard(synergy.resultCard);
                
                Debug.Log($"Applied synergy: {cardA.cardName} + {cardB.cardName} = {synergy.resultCard.cardName}");
            }
        }
    }
}
