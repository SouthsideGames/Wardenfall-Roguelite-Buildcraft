using System.Collections;
using UnityEngine;

public class ExplodeEnemy : Enemy
{
    [Header("EXPLODER SETTINGS:")]
    [SerializeField] private float explosionRadius = 3f;
    [SerializeField] private int explosionDamage = 15;
    [SerializeField] private float triggerDelay = 1.5f;
    [SerializeField] private GameObject explosionPrefab;
    [SerializeField] private GameObject damageAreaVisual;

    private bool hasTriggered = false;
    private bool isExploding = false;

    private EnemyAnimator animator;

    protected override void Start()
    {
        base.Start();
        animator = GetComponent<EnemyAnimator>();
        damageAreaVisual.SetActive(false);
    }

    protected override void Update()
    {
        base.Update();

        if (!hasSpawned || isExploding) return;

        float distanceToPlayer = Vector2.Distance(transform.position, character.transform.position);

        if (!hasTriggered && distanceToPlayer <= playerDetectionRadius)
        {
            StartCoroutine(TriggerExplosionSequence());
        }

        if (!hasTriggered)
        {
            movement.FollowCurrentTarget();
            animator?.PlayGroggyMove();
        }
    }

    private IEnumerator TriggerExplosionSequence()
    {
        hasTriggered = true;
        isExploding = true;

        movement.canMove = false;
        movement.StopMoving();
        damageAreaVisual.SetActive(true);

        StartCoroutine(PulseWhileWaiting(triggerDelay));
        yield return new WaitForSeconds(triggerDelay);

        Explode();
    }

    private IEnumerator PulseWhileWaiting(float duration)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            animator?.PlayExplosionPulse(1.3f, 0.2f);
            yield return new WaitForSeconds(0.25f);
            elapsed += 0.25f;
        }
    }

    private void Explode()
    {

        if (explosionPrefab != null)
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, explosionRadius);

        foreach (var hit in hits)
        {
            if (hit.TryGetComponent<Enemy>(out Enemy enemy) && enemy != this)
                enemy.TakeDamage(explosionDamage, false);

            if (hit.TryGetComponent<CharacterManager>(out CharacterManager player))
                player.TakeDamage(explosionDamage);
        }

        Die();
    }

    

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
