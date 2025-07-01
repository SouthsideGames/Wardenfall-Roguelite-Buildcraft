using System.Collections;
using UnityEngine;

public class GravulonBoss : Boss
{
    [Header("STAGE SETTINGS")]
    [SerializeField] private float slamRange = 3f;
    [SerializeField] private int slamDamage = 25;
    [SerializeField] private float stunDuration = 1.5f;
    [SerializeField] private float slamCooldown = 6f;

    private EnemyMovement enemyMovement;
    private bool isSlamming;
    private float slamTimer;
    private Vector3 originalScale;

    protected override void InitializeBoss()
    {
        base.InitializeBoss();
        enemyMovement = GetComponent<EnemyMovement>();
        originalScale = transform.localScale;
        slamTimer = slamCooldown;
    }

    protected override void Update()
    {
        base.Update();

        if (!hasSpawned || isSlamming) return;

        slamTimer -= Time.deltaTime;

        if (slamTimer <= 0)
        {
            ExecuteStage();
            slamTimer = slamCooldown;
        }
        else
        {
            enemyMovement.FollowCurrentTarget();
        }
    }

    protected override void ExecuteStage()
    {
        if (GetHealthPercent() > 0.5f)
            ExecuteStageOne();
        else
            ExecuteStageTwo();
    }

    private float GetHealthPercent()
    {
        return (float)health / maxHealth;
    }

    protected override void ExecuteStageOne()
    {
        if (isSlamming) return;
        isSlamming = true;

        enemyMovement.DisableMovement(1.5f);

        // Anticipation animation
        LeanTween.color(gameObject, Color.red, 0.2f).setLoopPingPong(2);
        LeanTween.scale(gameObject, new Vector3(originalScale.x * 1.5f, originalScale.y * 0.7f, originalScale.z), 0.2f).setEaseInOutQuad();

        Invoke(nameof(PerformSlam), 0.5f);
    }

    private void PerformSlam()
    {
        // Impact animation
        LeanTween.scale(gameObject, originalScale * 1.2f, 0.1f).setEasePunch();
        DealShockwaveDamage(slamRange, slamDamage, stunDuration);

        Invoke(nameof(ResetAfterSlam), 0.3f);
    }

    protected override void ExecuteStageTwo()
    {
        if (isSlamming) return;
        isSlamming = true;

        enemyMovement.DisableMovement(2f);

        // Bigger anticipation
        LeanTween.color(gameObject, Color.yellow, 0.2f).setLoopPingPong(3);
        LeanTween.scale(gameObject, new Vector3(originalScale.x * 1.7f, originalScale.y * 0.6f, originalScale.z), 0.2f).setEaseInOutQuad();

        Invoke(nameof(PerformEarthquake), 0.5f);
    }

    private void PerformEarthquake()
    {
        // Big impact
        LeanTween.scale(gameObject, originalScale * 1.3f, 0.1f).setEasePunch();
        DealShockwaveDamage(slamRange * 1.5f, slamDamage * 2, stunDuration * 1.5f);

        Invoke(nameof(ResetAfterSlam), 0.3f);
    }

    private void ResetAfterSlam()
    {
        transform.localScale = originalScale;
        isSlamming = false;
        enemyMovement.EnableMovement();
    }

    private void DealShockwaveDamage(float range, int damage, float stun)
    {
        Collider2D[] hitObjects = Physics2D.OverlapCircleAll(transform.position, range);

        foreach (Collider2D hit in hitObjects)
        {
            if (hit.CompareTag("Player"))
                character.TakeDamage(damage);
            else if (hit.CompareTag("Enemy"))
            {
                Enemy enemy = hit.GetComponent<Enemy>();
                if (enemy != null)
                {
                    enemy.TakeDamage(damage, false);
                    enemy.GetComponent<EnemyMovement>().DisableMovement(stun);
                }
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, slamRange);
    }
}
