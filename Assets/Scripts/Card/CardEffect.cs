using System;
using System.Collections;
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

        // Perform the effect based on the enum
        switch (effectType)
        {
            case CardEffectType.Utility_EternalPause:
                StopAllMovement(duration);
                break;
            case CardEffectType.Damage_FireballBarrage:
                StartCoroutine(SpawnFireballs(duration));
                break;
            default:
                break;
            // Add other cases for different card effects
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
        foreach (Enemy enemy in FindObjectsByType<Enemy>(FindObjectsSortMode.None))
        {
            enemy.GetComponent<EnemyMovement>()?.DisableMovement(duration);
        }
    }

    private void ResumeAllMovement()
    {
        foreach (Enemy enemy in FindObjectsByType<Enemy>(FindObjectsSortMode.None))
        {
            enemy.GetComponent<EnemyMovement>()?.EnableMovement();
        }
    }

    private IEnumerator SpawnFireballs(float duration)
    {
        float interval = 0.5f; // Spawn fireballs every 0.5 seconds
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            SpawnFireball();
            elapsedTime += interval;
            yield return new WaitForSeconds(interval);
        }
    }

    private void SpawnFireball()
    {
        GameObject fireball = Instantiate(Resources.Load<GameObject>("Prefabs/Fireball"));
        fireball.transform.position = GetRandomSpawnPosition();
    }

    private Vector3 GetRandomSpawnPosition()
    {
        Vector3 randomOffset = new Vector3(UnityEngine.Random.Range(-5f, 5f), UnityEngine.Random.Range(-5f, 5f), 0);
        return CharacterManager.Instance.transform.position + randomOffset;
    }
}

