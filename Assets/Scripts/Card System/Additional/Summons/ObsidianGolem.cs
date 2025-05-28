using System.Collections;
using UnityEngine;

public class ObsidianGolem : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 1.5f;
    [SerializeField] private float slamInterval = 3f;
    [SerializeField] private float slamRadius = 2f;
    [SerializeField] private int slamDamage = 30;
    [SerializeField] private GameObject slamVFXPrefab;
    [SerializeField] private float tauntRadius = 6f;
    [SerializeField] private StatusEffect fearEffect;

    private float activeTime;

    public void Initialize(float duration)
    {
        activeTime = duration;
        StartCoroutine(SlamRoutine());
        StartCoroutine(TauntRoutine());
        Destroy(gameObject, activeTime);
    }

    private void Update()
    {
        Enemy target = FindClosestEnemy();
        if (target != null)
        {
            Vector3 direction = (target.transform.position - transform.position).normalized;
            transform.position += direction * moveSpeed * Time.deltaTime;
        }
    }

    private IEnumerator SlamRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(slamInterval);
            Slam();
        }
    }

    private void Slam()
    {
        Instantiate(slamVFXPrefab, transform.position, Quaternion.identity);

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, slamRadius);
        foreach (var hit in hits)
        {
            Enemy enemy = hit.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(slamDamage);
            }
        }
    }

    private IEnumerator TauntRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);
            TauntEnemies();
        }
    }

    private void TauntEnemies()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, tauntRadius);
        foreach (var hit in hits)
        {
            Enemy enemy = hit.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.status.ApplyEffect(fearEffect, 0); 
            }
        }
    }

    private Enemy FindClosestEnemy()
    {
        Enemy[] enemies = FindObjectsByType<Enemy>(FindObjectsSortMode.None);
        Enemy closest = null;
        float minDist = Mathf.Infinity;

        foreach (Enemy enemy in enemies)
        {
            float dist = Vector2.Distance(transform.position, enemy.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                closest = enemy;
            }
        }

        return closest;
    }
}
