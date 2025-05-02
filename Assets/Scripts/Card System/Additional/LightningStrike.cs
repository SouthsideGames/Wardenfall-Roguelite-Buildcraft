using UnityEngine;

public class LightningStrike : MonoBehaviour
{
    [SerializeField] private float radius = 1.5f;
    [SerializeField] private LayerMask enemyLayer;

    public void Initialize(float damage)
    {
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, radius, enemyLayer);
        foreach (var hit in hitEnemies)
        {
            if (hit.TryGetComponent(out Enemy enemy))
                enemy.TakeDamage((int)damage);
        }

        Destroy(gameObject);
    }
}
