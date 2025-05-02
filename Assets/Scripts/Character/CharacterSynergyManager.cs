using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CharacterSynergyManager : MonoBehaviour 
{
    [SerializeField] private List<CardSynergy> availableSynergies = new();
    public event System.Action<string> OnSynergyActivated;

    public List<(CardEffectType, CardSO)> GetSynergiesForCard(CardEffectType cardType)
    {
        List<(CardEffectType, CardSO)> synergies = new();
        foreach (var synergy in availableSynergies)
        {
            if (synergy.cardTypeA == cardType)
                synergies.Add((synergy.cardTypeB, synergy.resultCard));
            else if (synergy.cardTypeB == cardType)
                synergies.Add((synergy.cardTypeA, synergy.resultCard));
        }
        return synergies;
    }
    
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
                // Remove both cards and add synergy card
                characterCards.RemoveCard(cardA);
                characterCards.RemoveCard(cardB);
                characterCards.AddCard(synergy.resultCard);
                
                OnSynergyActivated?.Invoke($"{cardA.cardName} + {cardB.cardName} = {synergy.resultCard.cardName}");
            }
        }
    }
}