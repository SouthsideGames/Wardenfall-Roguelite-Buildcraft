using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireParticleDamage : MonoBehaviour
{
    private int burnDamage;
    private float burnDuration;
    private float burnInterval;

    private List<Enemy> affectedEnemies = new List<Enemy>();

    public void SetupBurn(int damage, float duration, float interval)
    {
        burnDamage = damage;
        burnDuration = duration;
        burnInterval = interval;
    }

    private void OnParticleCollision(GameObject other)
    {
        Enemy enemy = other.GetComponent<Enemy>();
        if (enemy != null && !affectedEnemies.Contains(enemy))
        {
            affectedEnemies.Add(enemy);
            ApplyBurn(enemy);
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
