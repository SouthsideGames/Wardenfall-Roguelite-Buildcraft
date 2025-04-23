
using UnityEngine;

public class Void : MonoBehaviour
{
    [SerializeField] private float pullForce = 5f;
    [SerializeField] private float duration = 5f;
    [SerializeField] private float radius = 5f;
    [SerializeField] private LayerMask enemyMask;

    private void Start()
    {
        Destroy(gameObject, duration);
    }

    public void Initialize(float force, float time)
    {
        pullForce = force;
        duration = time;
    }

    private void Update()
    {
        Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, radius, enemyMask);
        
        foreach (Collider2D enemyCollider in enemies)
        {
            if (enemyCollider.TryGetComponent<Enemy>(out var enemy))
            {
                if (enemy.GetComponent<EnemyMovement>() != null)
                {
                    Vector2 direction = (transform.position - enemy.transform.position).normalized;
                    enemy.transform.position += (Vector3)(direction * pullForce * Time.deltaTime);
                }
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
