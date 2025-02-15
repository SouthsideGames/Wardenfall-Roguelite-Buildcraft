using System;
using System.Collections;
using System.Collections.Generic;
using SouthsideGames.DailyMissions;
using UnityEngine;

public class EnemyStatus : MonoBehaviour
{
    private EnemyMovement movement;
    private Enemy enemy;
    private bool isStunned = false;
    public bool IsStunned => isStunned;
    private bool isSlowed = false;
    private bool isBurned = false;

    private float originalMoveSpeed;
    private float slowFactor;

    private List<Coroutine> activeEffects = new List<Coroutine>();

    private void Awake()
    {
        movement = GetComponent<EnemyMovement>();
        enemy = GetComponent<Enemy>();

        if (movement == null)
        {
            Debug.LogError("EnemyMovement script not found on " + gameObject.name);
        }
    }

    public void ApplyEffect(StatusEffect effect)
    {
        switch (effect.EffectType)
        {
            case StatusEffectType.Stun:
                ApplyStun(effect.Duration);
                break;
            case StatusEffectType.Drain:
                ApplyDrain((int)effect.Value, effect.Duration, effect.Interval);
                break;
            case StatusEffectType.Burn:
                int value = (int)effect.Value;
                MissionManager.Increment(MissionType.burnThemDown, value);
                ApplyBurn(value, effect.Duration, effect.Interval);
                break;
            case StatusEffectType.Freeze:
                ApplyFreeze(effect.Duration);
                break;
            case StatusEffectType.Poison:
                ApplyPoison((int)effect.Value, effect.Duration, effect.Interval);
                break;
            case StatusEffectType.Blind:
                ApplyBlind(effect.Duration, effect.Value);
                break;
            case StatusEffectType.Weaken:
                ApplyWeaken(effect.Duration, effect.Value);
                break;
            case StatusEffectType.Slow:
                ApplySlow(effect.Duration, effect.Value);
                break;
            case StatusEffectType.Confuse:
                ApplyConfuse(effect.Duration);
                break;
            case StatusEffectType.Paralyze:
                ApplyParalyze(effect.Duration);
                break;
            case StatusEffectType.Fear:
                ApplyFear(effect.Duration);
                break;
            case StatusEffectType.Silence:
                ApplySilence(effect.Duration);
                break;
        }
    }

    // Temporarily disables enemy movement and abilities, leaving them immobilized.
    private void ApplyStun(float duration)
    {
        if (isStunned) return;
        isStunned = true;

        movement.DisableMovement(duration);

        StartCoroutine(RemoveStun(duration));
    }

    private IEnumerator RemoveStun(float duration)
    {
        yield return new WaitForSeconds(duration);
        isStunned = false;
    }

    // Reduces enemy movement speed, making them easier to evade or target.
    private void ApplySlow(float duration, float percentage)
    {
        if (isSlowed) return;
        isSlowed = true;

        originalMoveSpeed = movement.moveSpeed;
        slowFactor = originalMoveSpeed * (1 - percentage);
        movement.moveSpeed = slowFactor;

        StartCoroutine(RemoveSlow(duration));
    }

    private IEnumerator RemoveSlow(float duration)
    {
        yield return new WaitForSeconds(duration);
        movement.moveSpeed = originalMoveSpeed;
        isSlowed = false;
    }

    // Gradually saps enemy health, applying small, repeated damage over a duration.
    private void ApplyDrain(int damage, float duration, float interval)
    {
        Coroutine drainCoroutine = StartCoroutine(DrainEffect(damage, duration, interval));
        activeEffects.Add(drainCoroutine);
    }

    private IEnumerator DrainEffect(int damage, float duration, float interval)
    {
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            enemy.TakeDamage(damage, false);
            yield return new WaitForSeconds(interval);
            elapsedTime += interval;
        }
    }

    private void ApplyBurn(int damage, float duration, float interval)
    {
        if (isBurned) return;
        isBurned = true;

        Coroutine burnCoroutine = StartCoroutine(BurnEffect(damage, duration, interval));
        activeEffects.Add(burnCoroutine);

        StartCoroutine(RemoveBurn(duration));
    }

    private IEnumerator BurnEffect(int damage, float duration, float interval)
    {
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            enemy.TakeDamage(damage, false);
            yield return new WaitForSeconds(interval);
            elapsedTime += interval;
        }
    }

    private IEnumerator RemoveBurn(float duration)
    {
        yield return new WaitForSeconds(duration);
        isBurned = false;
    }

    // Completely halts enemy movement and attacks, akin to a temporary paralysis.
    private void ApplyFreeze(float duration)
    {
        movement.DisableMovement(duration);
        StartCoroutine(EffectDuration(() => { }, () => movement.EnableMovement(), duration));
    }

    // Applies a toxic effect that steadily damages enemies over a set time.
    private void ApplyPoison(int damage, float duration, float interval)
    {
        StartCoroutine(PeriodicEffect(() => enemy.TakeDamage(damage, false), duration, interval));
    }

    // Reduces enemy accuracy, causing their attacks to miss more often.
    private void ApplyBlind(float duration, float accuracyReduction)
    {
        enemy.ModifyAccuracy(-accuracyReduction);
        StartCoroutine(EffectDuration(() => { }, () => enemy.ModifyAccuracy(accuracyReduction), duration));
    }

    // Lowers the enemy’s damage output, reducing the threat of their attacks.
    private void ApplyWeaken(float duration, float damageReduction)
    {
        enemy.ModifyDamage(-damageReduction);
        StartCoroutine(EffectDuration(() => { }, () => enemy.ModifyDamage(damageReduction), duration));
    }

    // Disorients enemies, causing them to wander aimlessly instead of attacking.
    private void ApplyConfuse(float duration)
    {
        enemy.SetTargetToOtherEnemies();
        StartCoroutine(EffectDuration(() => { }, () => enemy.ResetTarget(), duration));
    }

    // Prevents movement and attacks while paralyzed.
    private void ApplyParalyze(float duration)
    {
        movement.DisableMovement(duration);
        enemy.DisableAttacks();
        StartCoroutine(EffectDuration(() => { }, () => { movement.EnableMovement(); enemy.EnableAttacks(); }, duration));
    }

    // Forces enemies to run away from the player, breaking their attack focus.
    private void ApplyFear(float duration)
    {
        movement.SetRunAwayFromPlayer();
        StartCoroutine(EffectDuration(() => { }, movement.ResetMovement, duration));
    }


    // Prevents enemies from using special abilities or skills temporarily.
    private void ApplySilence(float duration)
    {
        enemy.DisableAbilities();
        StartCoroutine(EffectDuration(() => { }, () => enemy.EnableAbilities(), duration));
    }

    private IEnumerator EffectDuration(Action onStart, Action onEnd, float duration)
    {
        onStart?.Invoke();
        yield return new WaitForSeconds(duration);
        onEnd?.Invoke();
    }

    private IEnumerator PeriodicEffect(Action effectAction, float duration, float interval)
    {
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            effectAction?.Invoke();
            yield return new WaitForSeconds(interval);
            elapsedTime += interval;
        }
    }

    public void ClearAllEffects()
    {
        foreach (Coroutine effect in activeEffects)
        {
            StopCoroutine(effect);
        }
        activeEffects.Clear();

        isStunned = false;
        isSlowed = false;
        isBurned = false;
        movement.moveSpeed = originalMoveSpeed;
    }

}
