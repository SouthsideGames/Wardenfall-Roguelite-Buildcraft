using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeRiftZone : MonoBehaviour
{
    [SerializeField] private float radius = 3f;
    [SerializeField] private LayerMask enemyMask;
    [SerializeField] private LayerMask projectileMask;
    [SerializeField] private float updateRate = 0.25f; // how often we scan for new targets

    private float slowFactor;
    private float duration;

    private List<EnemyMovement> slowedEnemies = new();
    private List<Rigidbody2D> slowedProjectiles = new();

    public void Initialize(float slowMultiplier, float activeDuration)
    {
        slowFactor = slowMultiplier;
        duration = activeDuration;
        StartCoroutine(ZoneRoutine());
    }

    private IEnumerator ZoneRoutine()
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            ApplySlows();
            yield return new WaitForSeconds(updateRate);
            elapsed += updateRate;
        }

        ClearSlows();
        Destroy(gameObject);
    }

    private void ApplySlows()
    {
        Collider2D[] enemyHits = Physics2D.OverlapCircleAll(transform.position, radius, enemyMask);
        foreach (var hit in enemyHits)
        {
            var move = hit.GetComponent<EnemyMovement>();
            if (move != null && !slowedEnemies.Contains(move))
            {
                move.moveSpeed *= slowFactor;
                slowedEnemies.Add(move);
            }
        }

        Collider2D[] projectileHits = Physics2D.OverlapCircleAll(transform.position, radius, projectileMask);
        foreach (var hit in projectileHits)
        {
            var rb = hit.GetComponent<Rigidbody2D>();
            if (rb != null && !slowedProjectiles.Contains(rb))
            {
                rb.linearVelocity *= slowFactor;
                slowedProjectiles.Add(rb);
            }
        }
    }

    private void ClearSlows()
    {
        foreach (var enemy in slowedEnemies)
        {
            if (enemy != null)
                enemy.moveSpeed /= slowFactor;
        }

        foreach (var proj in slowedProjectiles)
        {
            if (proj != null)
                proj.linearVelocity /= slowFactor;
        }
    }
}
