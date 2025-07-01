using System.Collections;
using UnityEngine;

public class TwinfangBoss : Boss
{
    [Header("STAGE 1 SETTINGS")]
    [SerializeField] private float attackRange = 3f;

    private EnemyMovement enemyMovement;
    private bool isAttacking;
    private Vector3 originalScale;

    protected override void InitializeBoss()
    {
        base.InitializeBoss();
        enemyMovement = GetComponent<EnemyMovement>();
        originalScale = transform.localScale;
        attackTimer = attackCooldown;
    }

    protected override void Update()
    {
        base.Update();

        if (!hasSpawned || isAttacking) return;

        attackTimer -= Time.deltaTime;

        if (Vector2.Distance(transform.position, PlayerTransform.position) <= attackRange && attackTimer <= 0)
        {
            StartCoroutine(LungeAttack());
            attackTimer = attackCooldown;
        }
        else
            enemyMovement.FollowCurrentTarget();
    }

    private IEnumerator LungeAttack()
    {
        if (isAttacking) yield break;

        isAttacking = true;
        enemyMovement.DisableMovement(0.8f);

        // Anticipation animation
        LeanTween.color(gameObject, Color.red, 0.15f).setLoopPingPong(2);
        LeanTween.scale(gameObject, originalScale * 1.3f, 0.2f).setEaseInOutQuad();

        yield return new WaitForSeconds(0.3f);

        // Lunge forward
        Vector2 attackDirection = (PlayerTransform.position - transform.position).normalized;
        yield return StartCoroutine(PerformLunge(attackDirection));

        // Impact animation
        LeanTween.scale(gameObject, originalScale * 1.1f, 0.1f).setEasePunch();

        yield return new WaitForSeconds(0.2f);

        // Retreat with animation
        LeanTween.color(gameObject, Color.white, 0.1f);
        yield return StartCoroutine(PerformRetreat(attackDirection));

        LeanTween.scale(gameObject, originalScale, 0.1f).setEaseOutQuad();

        isAttacking = false;
    }

    private IEnumerator PerformLunge(Vector2 direction)
    {
        Vector2 startPos = transform.position;
        Vector2 lungePos = startPos + direction * attackRange;

        float elapsed = 0f;
        while (elapsed < 0.25f)
        {
            transform.position = Vector2.Lerp(startPos, lungePos, elapsed / 0.25f);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = lungePos;
    }

    private IEnumerator PerformRetreat(Vector2 direction)
    {
        Vector2 startPos = transform.position;
        Vector2 retreatPos = startPos - direction * (attackRange * 0.75f);

        float elapsed = 0f;
        while (elapsed < 0.25f)
        {
            transform.position = Vector2.Lerp(startPos, retreatPos, elapsed / 0.25f);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = retreatPos;
    }
}
