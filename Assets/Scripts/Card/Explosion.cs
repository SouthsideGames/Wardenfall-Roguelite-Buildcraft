using UnityEngine;

public class Explosion : MonoBehaviour
{
   [SerializeField] private float radius;
    [SerializeField] private LayerMask enemyLayer;
    private int damage;

    public void SetDamage(int damageValue)
    {
        damage = damageValue;
    }

    private void Start()
    {
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, radius, enemyLayer);

        foreach (Collider2D enemy in hitEnemies)
        {
            Enemy enemyScript = enemy.GetComponent<Enemy>();
            if (enemyScript != null)
            {
                enemyScript.TakeDamage(damage, false);
            }
        }

        Destroy(gameObject); // Remove explosion effect after execution
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}

