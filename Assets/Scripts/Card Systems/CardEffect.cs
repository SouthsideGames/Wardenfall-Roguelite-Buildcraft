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

    public void ActivateEffect(CardEffectType effectType, float duration, CardSO _cardSO)
    {
        if (activeEffects.ContainsKey(effectType))
            return;

        ICardEffect effect = CardEffectFactory.GetEffect(effectType, _cardSO);
        if (effect != null)
        {
            activeEffects[effectType] = effect;
            effect.Activate(duration);
        }
        else
        {
            Debug.LogWarning($"Effect creation failed for type: {effectType}");
        }
    }

    public void DisableEffect(CardEffectType effectType)
    {
        if (!activeEffects.ContainsKey(effectType))
            return;

        activeEffects[effectType].Disable();
        activeEffects.Remove(effectType);
    }

    public bool IsEffectActive(CardEffectType effectType)
    {
        return activeEffects.ContainsKey(effectType);
    }
}

