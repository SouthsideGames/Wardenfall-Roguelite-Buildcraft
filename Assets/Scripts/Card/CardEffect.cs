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
        if(activeEffects.ContainsKey(effectType))
            return;

        ICardEffect effect = CardEffectFactory.GetEffect(effectType, _cardSO);
        if (effect != null)
        {
            activeEffects[effectType] = effect;
            effect.Activate(duration);

            // Check if the card has any synergy and apply the bonus if applicable
            if (_cardSO != null && _cardSO.HasSynergy)
            {
                foreach (var synergy in _cardSO.Synergies)
                {
                    if (IsEffectActive(synergy.EffectType))
                    {
                        ApplySynergyEffect(synergy.EffectType, synergy.SynergyBonus);
                    }
                }
            }
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

    private void ApplySynergyEffect(CardEffectType synergyEffectType, float synergyBonus)
    {
        if (activeEffects.TryGetValue(synergyEffectType, out ICardEffect effect))
        {
            effect.ApplySynergy(synergyBonus);
            Debug.Log($"Applying synergy effect: {synergyEffectType} with bonus: {synergyBonus}");
        }
    }
}

