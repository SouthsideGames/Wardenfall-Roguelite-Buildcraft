using System.Collections;
using UnityEngine;

public class ExplodeEnemy : Enemy
{
    [Header("EXPLODER SPECIFICS:")]
    [SerializeField] private float explosionRadius = 3f;
    [SerializeField] private int explosionDamage = 10;
    [SerializeField] private ParticleSystem explosionEffect;

    private bool isExploding = false;
    private bool isPlayerTooClose = false;

    private float panicTimer;
    private float panicIntensity = 0f;

    private EnemyAnimator animator;

    protected override void Start()
    {
        base.Start();
        animator = GetComponent<EnemyAnimator>();

        if (character == null)
            character = FindObjectOfType<CharacterManager>();

        StartCoroutine(PanicBehavior());
    }

    protected override void Update()
    {
        base.Update();

        if (!hasSpawned || isExploding)
            return;

        isPlayerTooClose = IsPlayerTooClose();

        if (isPlayerTooClose)
        {
            movement.StopMoving();
            StartExplosionSequence();
        }
        else if (CanAttack())
        {
            movement.FollowCurrentTarget();
            animator?.PlayGroggyMove();
        }
    }

    private bool IsPlayerTooClose()
    {
        float distance = Vector2.Distance(transform.position, character.transform.position);
        Debug.Log($"[ExplodeEnemy] Player distance: {distance}, threshold: {playerDetectionRadius}");
        return distance <= playerDetectionRadius;
    }


    private void StartExplosionSequence()
    {
        if (isExploding) return;
            isExploding = true;

        Debug.Log("[ExplodeEnemy] Explosion sequence started.");

        animator?.ResetVisual();

        GameObject visual = animator?.GetVisual()?.gameObject;
        if (visual != null)
        {
            LeanTween.scale(visual, Vector3.one * 1.5f, 0.12f)
                .setEaseOutBack()
                .setOnComplete(() =>
                {
                    StartCoroutine(DelayedExplosion());
                });
        }
        else
        {
            StartCoroutine(DelayedExplosion());
        }
    }

    private IEnumerator DelayedExplosion()
    {
        Debug.Log("[ExplodeEnemy] Starting 0.5s delay before explosion.");
        yield return new WaitForSeconds(0.5f);
        DoExplosion();
    }


    private void DoExplosion()
    {
        Debug.Log("[ExplodeEnemy] BOOM - Executing explosion logic.");

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
            float distanceToPlayer = Vector2.Distance(transform.position, character.transform.position);

            if (distanceToPlayer < 5f)
            {
                panicTimer += Time.deltaTime;
                panicIntensity = Mathf.Min(panicTimer / 2f, 1f);
                animator?.PlayPanicGrow(panicIntensity);
            }
            else
            {
                panicTimer = Mathf.Max(0f, panicTimer - Time.deltaTime * 2f);
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

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, playerDetectionRadius);
    }
}
