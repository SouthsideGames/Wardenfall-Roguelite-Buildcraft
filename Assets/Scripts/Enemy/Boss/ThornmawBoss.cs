using System.Collections;
using UnityEngine;

public class ThornmawBoss : Boss
{
    [Header("ATTACK SETTINGS")]
    [SerializeField] private float chargeForce = 15f;
    [SerializeField] private int spikeCount = 5;
    [SerializeField] private GameObject spikePrefab;

    private EnemyMovement enemyMovement;
    private Rigidbody2D rb;
    private Vector3 originalScale;
    private bool isAttacking;

    protected override void InitializeBoss()
    {
        base.InitializeBoss();
        enemyMovement = GetComponent<EnemyMovement>();
        rb = GetComponent<Rigidbody2D>();
        originalScale = transform.localScale;
        attackTimer = attackCooldown;
    }

    protected override void Update()
    {
        base.Update();

        if (!hasSpawned || isAttacking) return;

        attackTimer -= Time.deltaTime;

        if (attackTimer <= 0)
        {
            StartCoroutine(RollingCharge());
            attackTimer = attackCooldown;
        }
        else
        {
            enemyMovement.FollowCurrentTarget();
        }
    }

    private IEnumerator RollingCharge()
    {
        isAttacking = true;
        enemyMovement.DisableMovement(2f);

        // Anticipation animation
        LeanTween.color(gameObject, Color.yellow, 0.2f).setLoopPingPong(2);
        LeanTween.scale(gameObject, originalScale * 1.2f, 0.2f).setEaseInOutQuad();

        yield return new WaitForSeconds(0.4f);

        // Charge movement
        Vector2 chargeDirection = (PlayerTransform.position - transform.position).normalized;
        rb.AddForce(chargeDirection * chargeForce, ForceMode2D.Impulse);

        // Spawn spikes along the way
        for (int i = 0; i < spikeCount; i++)
        {
            Vector2 offset = Random.insideUnitCircle * 0.5f;
            Instantiate(spikePrefab, (Vector2)transform.position + offset, Quaternion.identity);
        }

        LeanTween.scale(gameObject, originalScale * 1.05f, 0.1f).setEasePunch();

        yield return new WaitForSeconds(1.5f);

        LeanTween.scale(gameObject, originalScale, 0.1f).setEaseOutQuad();
        isAttacking = false;
    }
}
