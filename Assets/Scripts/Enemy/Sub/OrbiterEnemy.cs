using UnityEngine;

public class OrbiterEnemy : Enemy
{
    [Header("Orbit Settings")]
    [SerializeField] private float orbitRadius = 3f;
    [SerializeField] private float baseOrbitSpeed = 120f;
    [SerializeField] private float detectionRange = 8f;
    [SerializeField] private LayerMask enemyLayer;

    private Enemy targetEnemy;
    private float currentAngle;
    private bool isIdle = false;
    private EnemyAnimator enemyAnimator;

    protected override void Start()
    {
        base.Start();

        currentAngle = Random.Range(0f, 360f); 
        enemyAnimator = GetComponent<EnemyAnimator>();
        FindStrongestNearbyEnemy();
    }

    protected override void Update()
    {
        base.Update();

        if (!hasSpawned) return;

        if (targetEnemy == null || !targetEnemy.gameObject.activeInHierarchy)
        {
            FindStrongestNearbyEnemy();
        }

        if (targetEnemy != null)
        {
            if (isIdle)
            {
                isIdle = false;
                enemyAnimator?.ResetVisual();
            }

            OrbitAroundTarget();
        }
        else
        {
            if (!isIdle)
            {
                isIdle = true;
                enemyAnimator?.PlayIdlePulse();
            }
        }
    }

    private void FindStrongestNearbyEnemy()
    {
        Collider2D[] nearbyEnemies = Physics2D.OverlapCircleAll(transform.position, detectionRange, enemyLayer);
        Enemy strongestEnemy = null;
        int highestHealth = 0;

        foreach (Collider2D col in nearbyEnemies)
        {
            Enemy enemy = col.GetComponent<Enemy>();
            if (enemy != null && enemy != this && enemy.maxHealth > highestHealth)
            {
                strongestEnemy = enemy;
                highestHealth = enemy.maxHealth;
            }
        }

        if (strongestEnemy != null && strongestEnemy != targetEnemy)
        {
            targetEnemy = strongestEnemy;

            enemyAnimator?.PlayPrePulseShake();
        }
    }

    private void OrbitAroundTarget()
    {
        float healthFactor = Mathf.Clamp01(targetEnemy.maxHealth / 100f);
        float adjustedSpeed = baseOrbitSpeed * (1f - 0.5f * healthFactor); 

        currentAngle += adjustedSpeed * Time.deltaTime;

        Vector2 offset = new Vector2(
            Mathf.Cos(currentAngle * Mathf.Deg2Rad),
            Mathf.Sin(currentAngle * Mathf.Deg2Rad)
        ) * orbitRadius;

        Vector2 targetPosition = (Vector2)targetEnemy.transform.position + offset;
        transform.position = Vector2.Lerp(transform.position, targetPosition, Time.deltaTime * 5f);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        if (targetEnemy != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(targetEnemy.transform.position, orbitRadius);
        }
    }
}
