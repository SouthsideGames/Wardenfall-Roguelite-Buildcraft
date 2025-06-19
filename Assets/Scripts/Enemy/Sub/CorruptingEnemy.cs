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

        // Optional: start idle pulse animation
        animator?.PlayIdlePulseAnimation();
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
        // Telegraph: small shake and glow before pulse
        animator?.PlayPrePulseShake();

        yield return new WaitForSeconds(0.4f); // Delay before actual pulse

        ApplyRandomStatusEffects();
        animator?.PlayCorruptionPulseAnimation();

        if (corruptionPulseEffect != null)
        {
            corruptionPulseEffect.Play();
        }
    }

    private void ApplyRandomStatusEffects()
    {
        Collider2D[] targets = Physics2D.OverlapCircleAll(transform.position, corruptionRadius, targetLayer);

        foreach (var target in targets)
        {
            if (target.TryGetComponent(out IStatusReceiver receiver))
            {
                int roll = Random.Range(0, 3);
                switch (roll)
                {
                    case 0: receiver.ApplyStatus(StatusType.Burn, 3f); break;
                    case 1: receiver.ApplyStatus(StatusType.Freeze, 2f); break;
                    case 2: receiver.ApplyStatus(StatusType.Drain, 4f); break;
                }
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, corruptionRadius);
    }
}