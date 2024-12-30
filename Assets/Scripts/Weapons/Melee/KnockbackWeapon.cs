using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnockbackWeapon : MeleeWeapon
{
    [Header("KNOCKBACK SETTINGS:")]
    [SerializeField] private float knockbackForce = 2f;
    [SerializeField] private float bounceDistance = 1f;

    protected override void AttackLogic()
    {
        base.AttackLogic();

        foreach (Enemy enemy in damagedEnemies)
        {
            ApplyKnockback(enemy);
        }
    }

    private void ApplyKnockback(Enemy enemy)
    {
        Vector2 direction = (enemy.transform.position - transform.position).normalized;
        EnemyMovement movement = enemy.GetComponent<EnemyMovement>();
        if (movement != null)
        {
            movement.ApplyKnockback(direction, knockbackForce, 0.2f);
        }
    }

    protected override void StartAttack()
    {
        base.StartAttack();
        damagedEnemies.Clear();
    }

    protected override void EndAttack()
    {
        base.EndAttack();
    }

}
