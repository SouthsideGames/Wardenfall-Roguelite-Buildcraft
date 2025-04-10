using System.Collections;
using UnityEngine;

public class GravityField : MonoBehaviour
{
     [SerializeField] private float pullStrength = 5f;
    private int damage;
    private float duration;
    private float pullRadius;

    public void Configure(int damage, float duration, float pullRadius)
    {
        this.damage = damage;
        this.duration = duration;
        this.pullRadius = pullRadius;

        CoroutineRunner.Instance.StartCoroutine(ActivateGravity()); 
    }

    private IEnumerator ActivateGravity()
    {
        float timer = 0f;

        while (timer < duration)
        {
            Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, pullRadius, LayerMask.GetMask("Enemy"));
            foreach (Collider2D enemyCollider in enemies)
            {
                Enemy enemy = enemyCollider.GetComponent<Enemy>();
                if (enemy != null)
                {
                    Vector2 direction = (transform.position - enemy.transform.position).normalized;
                    enemy.transform.position += (Vector3)direction * pullStrength * Time.deltaTime;
                }
            }

            timer += Time.deltaTime;
            yield return null;
        }

        ApplyDamageToEnemies();
        Destroy(gameObject);
    }

    private void ApplyDamageToEnemies()
    {
        Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, pullRadius, LayerMask.GetMask("Enemy"));
        foreach (Collider2D enemyCollider in enemies)
        {
            Enemy enemy = enemyCollider.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage, false);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, pullRadius);
    }
}
