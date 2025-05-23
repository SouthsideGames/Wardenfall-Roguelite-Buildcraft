using System;
using System.Collections;
using System.Collections.Generic;
using SouthsideGames.DailyMissions;
using UnityEngine;

public class EnemyStatus : MonoBehaviour
{
    private EnemyMovement movement;
    private Enemy enemy;
    private float originalMoveSpeed;
    private float slowFactor;

    private Dictionary<StatusEffectType, ActiveEffect> activeEffects = new();
    private StatusEffectUIManager statusUI;

    [SerializeField] private List<StatusEffectType> immunities = new List<StatusEffectType>();
    [SerializeField] private float statusResistance = 0f;

    public bool IsImmuneToEffect(StatusEffectType effectType) => immunities.Contains(effectType);

    private void Awake()
    {
        movement = GetComponent<EnemyMovement>();
        enemy = GetComponent<Enemy>();
        statusUI = GetComponent<StatusEffectUIManager>();

        if (movement == null)
            Debug.LogError("EnemyMovement script not found on " + gameObject.name);

        originalMoveSpeed = movement.moveSpeed;
    }

    public void ApplyEffect(StatusEffect effect, float delay = 0f)
    {
        if (delay > 0f)
        {
            StartCoroutine(DelayedApply(effect, delay));
            return;
        }

        // Check immunity
        if (IsImmuneToEffect(effect.EffectType))
            return;

        effect.ApplyStatModifiers(1.0f, statusResistance);

        if (activeEffects.TryGetValue(effect.EffectType, out var existing))
        {
            existing.Effect.ApplyStack();
            return;
        }

        ActiveEffect newEffect = new(effect);
        Coroutine coroutine = StartCoroutine(HandleEffect(newEffect));
        newEffect.Coroutine = coroutine;
        activeEffects[effect.EffectType] = newEffect;

        switch (effect.EffectType)
        {
            case StatusEffectType.Stun:
                movement.DisableMovement(effect.Duration);
                MissionManager.Increment(MissionType.stunEnemies, 1);
                break;
            case StatusEffectType.Burn:
                MissionManager.Increment(MissionType.burnThemDown, (int)effect.Value);
                break;
            case StatusEffectType.Slow:
                slowFactor = movement.moveSpeed * (1 - effect.Value);
                movement.moveSpeed = slowFactor;
                MissionManager.Increment(MissionType.slowEnemies, 1);
                break;
            case StatusEffectType.Weaken:
                enemy.modifierHandler.ModifyDamage(-effect.Value);
                MissionManager.Increment(MissionType.weakenEnemies, 1);
                break;
            case StatusEffectType.Confuse:
                enemy.SetTargetToOtherEnemies();
                MissionManager.Increment(MissionType.confuseEnemies, 1);
                break;
            case StatusEffectType.Paralyze:
                movement.DisableMovement(effect.Duration);
                enemy.DisableAttacks();
                MissionManager.Increment(MissionType.paralyzeEnemies, 1);
                break;
            case StatusEffectType.Fear:
                movement.SetRunAwayFromPlayer();
                MissionManager.Increment(MissionType.fearEnemies, 1);
                break;
        }

        statusUI?.AddOrUpdateEffect(effect);
    }

    private IEnumerator DelayedApply(StatusEffect effect, float delay)
    {
        yield return new WaitForSeconds(delay);
        ApplyEffect(effect);
    }

    private IEnumerator HandleEffect(ActiveEffect activeEffect)
    {
        var effect = activeEffect.Effect;

        effect.OnResetTimer = () => activeEffect.RemainingDuration = effect.Duration;

        float intervalTimer = 0f;

        while (activeEffect.RemainingDuration > 0)
        {
            activeEffect.RemainingDuration -= Time.deltaTime;
            intervalTimer += Time.deltaTime;

            if (effect.Interval > 0 && intervalTimer >= effect.Interval)
            {
                intervalTimer = 0f;
                ApplyTick(effect);
            }

            yield return null;
        }

        RemoveEffect(effect.EffectType);
    }

    private void ApplyTick(StatusEffect effect)
    {
        switch (effect.EffectType)
        {
            case StatusEffectType.Burn:
            case StatusEffectType.Drain:
            case StatusEffectType.Poison:
                enemy.TakeDamage((int)effect.Value, false);
                break;
        }
    }

    private void RemoveEffect(StatusEffectType type)
    {
        if (!activeEffects.TryGetValue(type, out var active)) return;

        StopCoroutine(active.Coroutine);
        activeEffects.Remove(type);

        switch (type)
        {
            case StatusEffectType.Stun:
                // auto handled
                break;
            case StatusEffectType.Slow:
                movement.moveSpeed = originalMoveSpeed;
                break;
            case StatusEffectType.Weaken:
                enemy.modifierHandler?.ModifyDamage(+active.Effect.Value);
                break;
            case StatusEffectType.Confuse:
                enemy.ResetTarget();
                break;
            case StatusEffectType.Paralyze:
                movement.EnableMovement();
                enemy.EnableAttacks();
                break;
            case StatusEffectType.Fear:
                movement.ResetMovement();
                break;
        }

        statusUI?.RemoveEffect(type);
    }

    public void ClearAllEffects()
    {
        foreach (var effect in activeEffects.Values)
        {
            if (effect.Coroutine != null)
                StopCoroutine(effect.Coroutine);
        }

        activeEffects.Clear();
        movement.moveSpeed = originalMoveSpeed;

        statusUI?.ClearAll();
    }

    private class ActiveEffect
    {
        public StatusEffect Effect;
        public float RemainingDuration;
        public Coroutine Coroutine;

        public ActiveEffect(StatusEffect effect)
        {
            Effect = effect;
            RemainingDuration = effect.Duration;
            Effect.OnResetTimer = () => RemainingDuration = effect.Duration;
        }
    }

    public bool IsStunned => activeEffects.ContainsKey(StatusEffectType.Stun);
    public bool IsSlowed => activeEffects.ContainsKey(StatusEffectType.Slow);
    public bool IsBurned => activeEffects.ContainsKey(StatusEffectType.Burn);
    public bool HasAnyEffect() => activeEffects.Count > 0;
}
