using System.Collections;
using UnityEngine;

public class CorruptingEnemy : Enemy
{
    [Header("CORRUPTION SETTINGS")]
    [SerializeField] private float corruptionInterval = 5f;
    [SerializeField] private float corruptionRadius = 4f;
    [SerializeField] private LayerMask targetLayer;
    [SerializeField] private ParticleSystem corruptionPulseEffect;

    private float corruptionTimer;
    private EnemyAnimator animator;
    private int tier;

    protected override void Start()
    {
        base.Start();

        animator = GetComponent<EnemyAnimator>();
        corruptionTimer = corruptionInterval;
        tier = enemyData != null ? enemyData.tier : 1;

        if (animator != null)
        {
            animator.PlayIdlePulseAnimation();
        }
    }

    private void Update()
    {
        if (!hasSpawned) return;

        corruptionTimer -= Time.deltaTime;
        if (corruptionTimer <= 0f)
        {
            StartCoroutine(CorruptionPulse());
            corruptionTimer = corruptionInterval;
        }
    }

    private IEnumerator CorruptionPulse()
    {
        animator?.PlayPrePulseShake();

        yield return new WaitForSeconds(0.4f);

        ApplyWeightedStatusEffects();

        animator?.PlayCorruptionPulseAnimation();
        if (corruptionPulseEffect != null)
            corruptionPulseEffect.Play();
    }

    private void ApplyWeightedStatusEffects()
    {
        Collider2D[] targets = Physics2D.OverlapCircleAll(transform.position, corruptionRadius, targetLayer);

        foreach (var target in targets)
        {
            if (target.TryGetComponent(out IStatusReceiver receiver))
            {
                StatusEffectType status = GetWeightedStatus();
                float duration = GetStatusDuration(status);

                receiver.ApplyStatus(status, duration);

                // Show floating icon at target
                StatusEffectUIManager.Show(status, target.transform.position + Vector3.up * 1f);
            }
        }
    }

    private StatusEffectType GetWeightedStatus()
    {
        int roll = Random.Range(0, 100);

        // Weighted by enemy tier
        return tier switch
        {
            1 => roll < 60 ? StatusEffectType.Burn : (roll < 90 ? StatusEffectType.Freeze : StatusEffectType.Drain),
            2 => roll < 40 ? StatusEffectType.Burn : (roll < 75 ? StatusEffectType.Freeze : StatusEffectType.Drain),
            _ => roll < 30 ? StatusEffectType.Burn : (roll < 60 ? StatusEffectType.Freeze : StatusEffectType.Drain),
        };
    }

    private float GetStatusDuration(StatusEffectType status)
    {
        return status switch
        {
            StatusEffectType.Burn => 3f,
            StatusEffectType.Freeze => 2f,
            StatusEffectType.Drain => 4f,
            _ => 3f,
        };
    }

    protected override void OnDeath()
    {
        base.OnDeath();
        StartCoroutine(CorruptionPulse()); // Final burst on death
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0.7f, 0f, 0.9f, 0.6f);
        Gizmos.DrawWireSphere(transform.position, corruptionRadius);
    }
}