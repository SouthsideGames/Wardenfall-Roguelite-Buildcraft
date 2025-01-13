using System.Collections.Generic;
using UnityEngine;

public class CardEffect : MonoBehaviour
{
    public static CardEffect Instance;
    private Dictionary<CardEffectType, float> activeEffects = new Dictionary<CardEffectType, float>();

    private void Awake()
    {
        if(Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }


    public void ActivateEffect(CardEffectType effectType, float duration)
    {
        if (activeEffects.ContainsKey(effectType))
        {
            Debug.LogWarning($"Effect {effectType} is already active!");
            return;
        }

        activeEffects[effectType] = duration;

        switch (effectType)
        {
            case CardEffectType.Utility_EternalPause:
                StopAllMovement(duration);
                break;
            default:
                break;

        }

        Debug.Log($"{effectType} activated for {duration} seconds.");
    }

    public void DisableEffect(CardEffectType effectType)
    {
        if (!activeEffects.ContainsKey(effectType))
            return;

        activeEffects.Remove(effectType);

        // Perform cleanup logic based on the enum
        switch (effectType)
        {
            case CardEffectType.Utility_EternalPause:
                ResumeAllMovement();
                break;

            // Add cleanup for other effects
        }

        Debug.Log($"{effectType} disabled.");
    }

    private void Update()
    {
        // Decrease the timers for active effects
        List<CardEffectType> toDisable = new List<CardEffectType>();
        foreach (var effect in activeEffects)
        {
            activeEffects[effect.Key] -= Time.deltaTime;
            if (activeEffects[effect.Key] <= 0)
                toDisable.Add(effect.Key);
        }

        // Disable effects whose timers have expired
        foreach (var effectType in toDisable)
            DisableEffect(effectType);
    }

    private void StopAllMovement(float duration)
    {
        foreach (Enemy enemy in Object.FindObjectsByType<Enemy>(FindObjectsSortMode.None))
        {
            enemy.GetComponent<EnemyMovement>()?.DisableMovement(duration);
        }
    }

    private void ResumeAllMovement()
    {
        foreach (Enemy enemy in Object.FindObjectsByType<Enemy>(FindObjectsSortMode.None))
        {
            enemy.GetComponent<EnemyMovement>()?.EnableMovement();
        }
    }
}
