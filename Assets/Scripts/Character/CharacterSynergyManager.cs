
using UnityEngine;
using System.Collections.Generic;

public class CardSynergyManager : MonoBehaviour 
{
    public static CardSynergyManager Instance { get; private set; }
    [SerializeField] private List<CardSynergy> availableSynergies = new();
    public event System.Action<string> OnSynergyActivated;

    private void Awake()
    {
       if(Instance == null)
               Instance = this;
            else
                Destroy(gameObject);
    }

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
            
            foreach (var card in deck)
            {
                if (card.effectType == synergy.cardTypeA && cardA == null)
                    cardA = card;
                else if (card.effectType == synergy.cardTypeB && cardB == null)
                    cardB = card;
                
                if (cardA != null && cardB != null)
                    break;
            }
            
            if (cardA != null && cardB != null)
            {
                characterCards.RemoveCard(cardA);
                characterCards.RemoveCard(cardB);
                characterCards.AddCard(synergy.resultCard);
                
                OnSynergyActivated?.Invoke($"{cardA.cardName} + {cardB.cardName} = {synergy.resultCard.cardName}");
            }
        }
    }
}
