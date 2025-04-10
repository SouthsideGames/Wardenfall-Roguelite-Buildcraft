using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireWall : MonoBehaviour
{
    [SerializeField] private int burnDamage;
    [SerializeField] private float burnDuration;
    [SerializeField] private float burnInterval;

    private int damage;
    private float duration;

    private List<Enemy> affectedEnemies = new List<Enemy>();

    public void Setup(int _damage, float _duration)
    {
        damage = _damage;
        duration = _duration;
        CoroutineRunner.Instance.StartCoroutine(DestroyAfterDuration());
    }

    private IEnumerator DestroyAfterDuration()
    {
        yield return new WaitForSeconds(duration);
        Destroy(gameObject);
    }

    void OnParticleCollision(GameObject other)
    {
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
            StatusEffect burnEffect = new StatusEffect(StatusEffectType.Burn, burnDuration, burnDamage, burnInterval);
            status.ApplyEffect(burnEffect);
        }
    }
}
