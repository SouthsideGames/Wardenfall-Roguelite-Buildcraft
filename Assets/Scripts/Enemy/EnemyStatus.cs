using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStatus : MonoBehaviour
{
      private Enemy enemy;

    private Dictionary<StatusEffectType, StatusEffect> activeEffects = new();

    [Header("EFFECT SETTINGS")]
    [SerializeField] private float defaultInterval = 1.0f;

    private void Awake()
    {
        enemy = GetComponent<Enemy>();
    }

    public void ApplyEffect(StatusEffectType effectType, int damage, float duration, float interval = -1f)
    {
        if (!activeEffects.ContainsKey(effectType))
        {
            interval = interval > 0 ? interval : defaultInterval;
            StatusEffect newEffect = new(effectType, damage, duration, interval, enemy);
            activeEffects[effectType] = newEffect;
            StartCoroutine(HandleEffect(newEffect));
        }
    }

    private IEnumerator HandleEffect(StatusEffect effect)
    {
        float elapsedTime = 0;

        while (elapsedTime < effect.Duration && activeEffects.ContainsKey(effect.Type))
        {
            effect.ApplyDamage();
            elapsedTime += effect.Interval;
            yield return new WaitForSeconds(effect.Interval);
        }

        activeEffects.Remove(effect.Type);
    }

    public void RemoveEffect(StatusEffectType effectType)
    {
        if (activeEffects.ContainsKey(effectType))
        {
            activeEffects.Remove(effectType);
        }
    }
}
