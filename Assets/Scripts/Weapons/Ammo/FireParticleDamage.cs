using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireParticleDamage : MonoBehaviour
{

    private int damage;
    private int burnDamage;
    private float burnDuration;
    private float burnInterval;

    private List<Enemy> affectedEnemies = new List<Enemy>();

    public void SetupBurn(int baseDamage, int burnDmg, float duration, float interval)
    {
        damage = baseDamage;
        burnDamage = burnDmg;
        burnDuration = duration;
        burnInterval = interval;
    }

    void OnParticleCollision(GameObject other)
    {
        // Check if collided object has Enemy component
        Enemy enemy = other.GetComponent<Enemy>();

        if (enemy != null && !affectedEnemies.Contains(enemy))
        {
            // Apply initial damage
            enemy.TakeDamage(damage, false);

            // Apply burn effect using EnemyStatus
            ApplyBurn(enemy);

            // Add to affected list
            affectedEnemies.Add(enemy);
        }
    }

    private void ApplyBurn(Enemy enemy)
    {
        EnemyStatus status = enemy.GetComponent<EnemyStatus>();
        if (status != null)
        {
            status.ApplyEffect(StatusEffectType.Burn, burnDamage, burnDuration, burnInterval);
        }
    }
}

