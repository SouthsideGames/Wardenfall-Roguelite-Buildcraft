using System.Collections;
using UnityEngine;

public class ExplodeEnemy : Enemy
{
    [Header("EXPLODER SPECIFICS:")]
    [SerializeField] private float explosionRadius = 3f; 
    [SerializeField] private int explosionDamage = 10; 
    [SerializeField] private ParticleSystem explosionEffect;

    private bool isExploding = false; 
    private float panicTimer;
    private float panicIntensity = 0f;

    private EnemyAnimator animator;

    protected override void Start()
    {
        base.Start();
        animator = GetComponent<EnemyAnimator>();
        StartCoroutine(PanicBehavior());
    }

    protected override void Update()
    {
        base.Update();

        if (!hasSpawned || !CanAttack() || isExploding)
            return;

        if (IsPlayerTooClose())
        {
            StartExplosionSequence();
        }
        else
        {
            movement.FollowCurrentTarget();
            animator?.PlayGroggyMove();
        }
    }

    private bool IsPlayerTooClose()
    {
        return Vector2.Distance(transform.position, character.transform.position) <= playerDetectionRadius;
    }

    private void StartExplosionSequence()
    {
        if (isExploding) return;
        isExploding = true;

        animator?.ResetVisual();

        GameObject visual = animator?.GetVisual()?.gameObject;
        if (visual != null)
        {
            LeanTween.scale(visual, Vector3.one * 1.5f, 0.12f)
                .setEaseOutBack()
                .setOnComplete(DoExplosion);
        }
        else
        {
            DoExplosion();
        }
    }

    private void DoExplosion()
    {
        if (explosionEffect != null)
        {
            var effect = Instantiate(explosionEffect, transform.position, Quaternion.identity);
            effect.Play();
        }

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, explosionRadius);

        foreach (Collider2D hit in hits)
        {
            if (hit.TryGetComponent<Enemy>(out Enemy enemy) && enemy != this)
                enemy.TakeDamage(explosionDamage, false);

            if (hit.TryGetComponent<CharacterManager>(out CharacterManager player))
                player.TakeDamage(explosionDamage);
        }

        Die();
    }

    private IEnumerator PanicBehavior()
    {
        while (true)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);

            if (distanceToPlayer < 5f)
            {
                panicTimer += Time.deltaTime;
                panicIntensity = Mathf.Min(panicTimer / 2f, 1f); // Builds up over 2 seconds
                animator?.PlayPanicGrow(panicIntensity);
            }
            else
            {
                panicTimer = Mathf.Max(0f, panicTimer - Time.deltaTime * 2f); // Calms down faster
                panicIntensity = panicTimer / 2f;
                animator?.ResetVisual();
            }

            yield return null;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
