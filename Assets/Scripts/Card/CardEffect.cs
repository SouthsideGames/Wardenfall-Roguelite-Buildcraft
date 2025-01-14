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

    public void ActivateEffect(CardEffectType effectType, float duration)
    {
        if (activeEffects.ContainsKey(effectType))
        {
            Debug.LogWarning($"{effectType} is already active!");
            return;
        }

        ICardEffect effect = CardEffectFactory.GetEffect(effectType);
        if (effect != null)
        {
            activeEffects[effectType] = effect;
            effect.Activate(duration);
            Debug.Log($"{effectType} activated for {duration} seconds.");
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

