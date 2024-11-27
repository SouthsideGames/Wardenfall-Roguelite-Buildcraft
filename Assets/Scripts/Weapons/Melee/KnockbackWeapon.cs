using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnockbackWeapon : MeleeWeapon
{
     [Header("KNOCKBACK SETTINGS:")]
    [SerializeField] private float knockbackForce = 3f; // How far the enemy is knocked back
    [SerializeField] private float knockbackDuration = 0.2f; // How long the knockback effect lasts

    protected override void AttackLogic()
    {
        Collider2D[] enemies = Physics2D.OverlapBoxAll
        (
            hitpoint.position,
            hitCollider.bounds.size,
            hitpoint.localEulerAngles.z,
            enemyMask
        );

        for (int i = 0; i < enemies.Length; i++)
        {
            Enemy enemy = enemies[i].GetComponent<Enemy>();

            if (!damagedEnemies.Contains(enemy))
            {
                // Apply damage
                int damage = GetDamage(out bool isCriticalHit);
                enemy.TakeDamage(damage, isCriticalHit);

                // Apply knockback
                ApplyKnockback(enemy);

                damagedEnemies.Add(enemy);
            }
        }
    }

    private void ApplyKnockback(Enemy enemy)
    {
        EnemyMovement enemyMovement = enemy.GetComponent<EnemyMovement>();
        if (enemyMovement != null)
        {
            // Calculate direction away from the weapon
            Vector2 knockbackDirection = (enemy.transform.position - transform.position).normalized;

            // Apply knockback by moving the enemy directly
            Vector2 knockbackTarget = (Vector2)enemy.transform.position + knockbackDirection * knockbackForce;

            // Temporarily disable movement to simulate knockback
            enemyMovement.DisableMovement(knockbackDuration);

            // Smoothly move the enemy to the knockback position
            StartCoroutine(MoveEnemyToPosition(enemy, knockbackTarget, knockbackDuration));
        }
    }

    private IEnumerator MoveEnemyToPosition(Enemy enemy, Vector2 targetPosition, float duration)
    {
        float elapsed = 0f;
        Vector2 startPosition = enemy.transform.position;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            // Move the enemy smoothly
            enemy.transform.position = Vector2.Lerp(startPosition, targetPosition, t);

            yield return null;
        }
    }
}
