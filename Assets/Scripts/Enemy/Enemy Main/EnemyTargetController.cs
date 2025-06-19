using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Enemy))]
public class EnemyTargetController : MonoBehaviour
{
    private Enemy enemy;
    private EnemyMovement movement;

    private void Awake()
    {
        enemy = GetComponent<Enemy>();
        movement = GetComponent<EnemyMovement>();
    }

    public void SetTargetToPlayer()
    {
        enemy.PlayerTransform = enemy.Character?.transform;
        movement?.SetTarget(enemy.PlayerTransform);
    }

    public void SetTargetToOtherEnemies()
    {
        enemy.PlayerTransform = null;

        Enemy[] all = FindObjectsByType<Enemy>(FindObjectsSortMode.None);
        List<Enemy> validTargets = new();

        foreach (Enemy e in all)
            if (e != null && e != enemy)
                validTargets.Add(e);

        if (validTargets.Count > 0)
        {
            int index = UnityEngine.Random.Range(0, validTargets.Count);
            movement?.SetTarget(validTargets[index].transform);
        }
    }

    public Enemy FindClosestWoundedAlly(float radius)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(enemy.transform.position, radius);
        Enemy closest = null;
        float closestDist = Mathf.Infinity;

        foreach (var hit in hits)
        {
            if (hit.TryGetComponent(out Enemy ally) && ally != enemy)
            {
                if (ally.CurrentHealth < ally.MaxHealth)
                {
                    float dist = Vector2.Distance(enemy.transform.position, ally.transform.position);
                    if (dist < closestDist)
                    {
                        closest = ally;
                        closestDist = dist;
                    }
                }
            }
        }

        return closest;
    }
}
