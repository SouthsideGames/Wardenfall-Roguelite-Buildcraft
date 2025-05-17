using UnityEngine;

public class SwarmLeaderEnemy : Enemy 
{
    [SerializeField] private float buffRadius = 5f;
    [SerializeField] private float damageMultiplier = 1.5f;
    [SerializeField] private float speedMultiplier = 1.2f;
    
    private void Update()
    {
        Collider2D[] nearbyEnemies = Physics2D.OverlapCircleAll(transform.position, buffRadius, LayerMask.GetMask("Enemy"));
        
        foreach (Collider2D enemyCollider in nearbyEnemies)
        {
            Enemy enemy = enemyCollider.GetComponent<Enemy>();
            if (enemy != null && enemy != this)
            {
                enemy.SetDamageMultiplier(damageMultiplier);
                EnemyMovement movement = enemy.GetComponent<EnemyMovement>();
                if (movement != null)
                {
                    movement.SetSpeedMultiplier(speedMultiplier);
                }
            }
        }
    }
}
