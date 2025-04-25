using UnityEngine;
using System.Collections;

public class Decoy : MonoBehaviour
{
    private float lifetime;
    private float aggroRadius;

    public void Initialize(float activeTime, float radius)
    {
        lifetime = activeTime;
        aggroRadius = radius;

        StartCoroutine(HandleDecoyLifetime());
        RetargetEnemies();
    }

    private void RetargetEnemies()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, aggroRadius);

        foreach (Collider2D hit in hits)
        {
            EnemyMovement movement = hit.GetComponent<EnemyMovement>();
            if (movement != null)
            {
                movement.SetTarget(transform);
            }
        }
    }

    private IEnumerator HandleDecoyLifetime()
    {
        yield return new WaitForSeconds(lifetime);
        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, aggroRadius);
    }
} 