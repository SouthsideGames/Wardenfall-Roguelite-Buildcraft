using UnityEngine;

public class FlameCrawler : MonoBehaviour
{
    [SerializeField] private float speed = 4f;
    [SerializeField] private float explosionRadius = 2.5f;
    [SerializeField] private int damage = 20;
    [SerializeField] private GameObject explosionEffect;
    [SerializeField] private StatusEffect burnEffect;

    private Transform target;

    private void Start()
    {
        target = FindClosestEnemy();
    }

    private void Update()
    {
        if (target == null) return;

        transform.position = Vector2.MoveTowards(transform.position, target.position, speed * Time.deltaTime);

        if (Vector2.Distance(transform.position, target.position) < 0.5f)
        {
            Explode();
        }
    }

    private Transform FindClosestEnemy()
    {
        Enemy[] enemies = FindObjectsByType<Enemy>(FindObjectsSortMode.None);
        Transform closest = null;
        float minDist = Mathf.Infinity;

        foreach (Enemy enemy in enemies)
        {
            float dist = Vector2.Distance(transform.position, enemy.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                closest = enemy.transform;
            }
        }

        return closest;
    }

    private void Explode()
    {
        Instantiate(explosionEffect, transform.position, Quaternion.identity);

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
        foreach (var hit in hits)
        {
            Enemy enemy = hit.GetComponent<Enemy>();
            if (enemy)
            {
                enemy.TakeDamage(damage);
                enemy.status.ApplyEffect(burnEffect, 0);
            }
        }

        Destroy(gameObject);
    }
}
