using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CardEffect : MonoBehaviour
{
    public static CardEffect Instance;

    private Dictionary<CardEffectType, ICardEffect> activeEffects = new Dictionary<CardEffectType, ICardEffect>();

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void ActivateEffect(CardSO cardSO)
    {
        if (activeEffects.ContainsKey(cardSO.EffectType))
        {
            Debug.LogWarning($"{cardSO.EffectType} is already active!");
            return;
        }

        ICardEffect effect = CardEffectFactory.GetEffect(cardSO.EffectType, cardSO);
        if (effect != null)
        {
            activeEffects[cardSO.EffectType] = effect;
            effect.Activate(cardSO.ActiveTime);
            Debug.Log($"{cardSO.EffectType} activated for {cardSO.ActiveTime} seconds.");
        }
    }

    public void DisableEffect(CardEffectType effectType)
    {
        if (!activeEffects.ContainsKey(effectType))
            return;

        activeEffects[effectType].Disable();
        activeEffects.Remove(effectType);
        Debug.Log($"{effectType} disabled.");
    }
}

