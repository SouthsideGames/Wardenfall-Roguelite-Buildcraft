using System.Collections;
using System.Collections.Generic;
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
                ApplyBurn((int)effect.Value, effect.Duration, effect.Interval);
                break;
        }
    }

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

    private void ApplySlow(float duration, float percentage)
    {
        if (isSlowed) return;
        isSlowed = true;

        // Store original speed and apply slow effect
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
        if (isBurned) return; // Prevent multiple burns
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

    public void ClearAllEffects()
    {
        foreach (Coroutine effect in activeEffects)
        {
            StopCoroutine(effect);
        }
        activeEffects.Clear();

        // Reset states and speed
        isStunned = false;
        isSlowed = false;
        isBurned = false;
        movement.moveSpeed = originalMoveSpeed;
    }
}
