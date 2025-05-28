using System.Collections;
using UnityEngine;

public class TeamUp : MonoBehaviour
{
    [SerializeField] private float followDistance = 1.5f;
    [SerializeField] private float moveSpeed = 4f;
    [SerializeField] private float fireRate = 1f;
    [SerializeField] private float fireRange = 5f;
    [SerializeField] private GameObject fireballPrefab;
    [SerializeField] private Transform firePoint;

    private Transform player;

    public void Initialize(Transform playerTransform, float duration)
    {
        player = playerTransform;
        StartCoroutine(FireRoutine());
        Destroy(gameObject, duration);
    }

    private void Update()
    {
        if (player == null) return;

        Vector3 targetPos = player.position + Vector3.up * followDistance;
        transform.position = Vector3.Lerp(transform.position, targetPos, moveSpeed * Time.deltaTime);
    }

    private IEnumerator FireRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(fireRate);
            Enemy target = FindClosestEnemy();
            if (target != null)
            {
                FireAt(target);
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
            if (dist < minDist && dist <= fireRange)
            {
                closest = enemy;
                minDist = dist;
            }
        }

        return closest;
    }

    private void FireAt(Enemy target)
    {
        if (fireballPrefab != null && firePoint != null)
        {
            GameObject fireball = Instantiate(fireballPrefab, firePoint.position, Quaternion.identity);
            Vector2 direction = (target.transform.position - firePoint.position).normalized;
            fireball.GetComponent<Rigidbody2D>().linearVelocity = direction * 6f;
        }
    }
}
