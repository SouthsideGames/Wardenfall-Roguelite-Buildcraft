using UnityEngine;

public class OrbiterEnemy : Enemy
{
    [Header("Orbit Settings")]
    [SerializeField] private float orbitRadius = 3f;
    [SerializeField] private float orbitSpeed = 120f;
    [SerializeField] private float detectionRange = 8f;
    [SerializeField] private LayerMask enemyLayer;
    
    private Enemy targetEnemy;
    private float currentAngle;

    protected override void Start()
    {
        base.Start();
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
            OrbitAroundTarget();
        }
        else
        {
            movement.FollowCurrentTarget();
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

        targetEnemy = strongestEnemy;
        if (targetEnemy != null)
        {
            // Calculate initial angle based on current position
            Vector2 toTarget = transform.position - targetEnemy.transform.position;
            currentAngle = Mathf.Atan2(toTarget.y, toTarget.x) * Mathf.Rad2Deg;
        }
    }

    private void OrbitAroundTarget()
    {
        currentAngle += orbitSpeed * Time.deltaTime;
        
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
