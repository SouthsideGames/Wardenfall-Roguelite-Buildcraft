using UnityEngine;

public class AdaptiveBullet : BulletBase
{
    protected override void OnTriggerEnter2D(Collider2D collider)
    {
        if (IsInLayerMask(collider.gameObject.layer, enemyMask))
        {
            Enemy enemy = collider.GetComponent<Enemy>();
            if (enemy != null)
            {
                int adjustedDamage = CalculateDamage(enemy);
                Attack(enemy, adjustedDamage);
            }

            Release();
        }
    }

    private int CalculateDamage(Enemy enemy)
    {
        int adjustedDamage = damage;

        // Check if the enemy has the Boss script
        if (enemy.GetComponent<Boss>() != null)
        {
            adjustedDamage = Mathf.RoundToInt(damage * 2f);
        }

        return adjustedDamage;
    }

    private void Attack(Enemy enemy, int _damage)
    {
        enemy.TakeDamage(_damage, isCriticalHit);
    }
}
