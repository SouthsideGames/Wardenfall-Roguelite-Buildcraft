using System.Collections.Generic;
using UnityEngine;

public class WildcardProtocolEffect : MonoBehaviour, ICardEffect
{
    [SerializeField] private CardLibrary cardLibrary;

    public void Activate(CharacterManager target, CardSO originalCard)
    {
        if (target == null || cardLibrary == null)
        {
            Debug.LogWarning("WildcardProtocolEffect: Target or CardLibrary not available.");
            Destroy(gameObject);
            return;
        }

        List<CardSO> allCards = new List<CardSO>(cardLibrary.allCards);

        allCards.RemoveAll(card => card.cardID == originalCard.cardID || card.effectPrefab == null);

        if (allCards.Count == 0)
        {
            Debug.Log("Wildcard Protocol failed: no valid cards in the library.");
            Destroy(gameObject);
            return;
        }

        CardSO randomCard = allCards[Random.Range(0, allCards.Count)];
        GameObject effectInstance = Instantiate(randomCard.effectPrefab);
        ICardEffect effect = effectInstance.GetComponent<ICardEffect>();

        if (effect != null)
        {
            Debug.Log($"Wildcard Protocol triggered: {randomCard.cardName} effect activated.");
            effect.Activate(target, randomCard);
        }
        else
        {
            Debug.LogWarning("Wildcard Protocol: Selected card's prefab does not implement ICardEffect.");
            Destroy(effectInstance);
        }

        Destroy(gameObject);
    }

    public void Deactivate() { }
    public void Tick(float deltaTime) { }
} 
