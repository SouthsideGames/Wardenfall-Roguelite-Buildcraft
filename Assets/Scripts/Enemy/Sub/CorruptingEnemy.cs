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

    protected override void Start()
    {
        base.Start();
        animator = GetComponent<EnemyAnimator>();
        corruptionTimer = corruptionInterval;

        animator?.PlayIdlePulseAnimation();
    }

    protected override void Update()
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
            if (target.TryGetComponent(out EnemyStatus enemyStatus))
            {
                StatusEffectType statusType = GetWeightedStatus();
                float duration = GetStatusDuration(statusType);
                float value = GetStatusValue(statusType);
                float interval = statusType is StatusEffectType.Burn or StatusEffectType.Drain or StatusEffectType.Poison ? 1f : 0f;

                StatusEffect effect = new StatusEffect(
                    statusType,
                    duration,
                    value,
                    interval,
                    1,
                    StackBehavior.Extend
                );

                enemyStatus.ApplyEffect(effect);

                // Show floating icon through the UI component
                enemyStatus.GetComponent<EnemyStatusEffectUI>()?.ShowFloatingStatus(statusType);
            }
        }
    }

    private StatusEffectType GetWeightedStatus()
    {
        int roll = Random.Range(0, 100);
        if (roll < 50) return StatusEffectType.Burn;
        if (roll < 85) return StatusEffectType.Freeze;
        return StatusEffectType.Drain;
    }

    private float GetStatusDuration(StatusEffectType type)
    {
        return type switch
        {
            StatusEffectType.Burn => 3f,
            StatusEffectType.Freeze => 2f,
            StatusEffectType.Drain => 4f,
            _ => 3f,
        };
    }

    private float GetStatusValue(StatusEffectType type)
    {
        return type switch
        {
            StatusEffectType.Burn => 2f,
            StatusEffectType.Drain => 1.5f,
            _ => 0f,
        };
    }

    public override void Die()
    {
        base.Die();
        StartCoroutine(CorruptionPulse()); // Final corruption pulse
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0.7f, 0f, 0.9f, 0.6f);
        Gizmos.DrawWireSphere(transform.position, corruptionRadius);
    }
}
