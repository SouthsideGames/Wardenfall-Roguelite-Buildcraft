using UnityEngine;
using System.Collections;

public class DetonabotAI : MonoBehaviour
{
    private Enemy targetEnemy;
    private float moveSpeed = 3f;
    private float explosionRadius = 2.5f;
    private int explosionDamage = 9999; // Very High Damage

    [SerializeField] private GameObject explosionPrefab;
    [SerializeField] private float explosionDelay = 2f;

    public void Initialize(CardSO sourceCard)
    {
        FindHighHealthTarget();
    }

    void Update()
    {
        if (targetEnemy == null)
        {
            FindHighHealthTarget();
            return;
        }

        transform.position = Vector3.MoveTowards(transform.position, targetEnemy.transform.position, moveSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, targetEnemy.transform.position) < 0.5f)
        {
            StartCoroutine(ExplodeAfterDelay());
        }
    }

    void FindHighHealthTarget()
    {
        Enemy[] allEnemies = FindObjectsByType<Enemy>(FindObjectsSortMode.None);
        float maxHealth = -1f;

        foreach (var enemy in allEnemies)
        {
            if (enemy.CurrentHealth > maxHealth)
            {
                maxHealth = enemy.CurrentHealth;
                targetEnemy = enemy;
            }
        }
    }

    IEnumerator ExplodeAfterDelay()
    {
        yield return new WaitForSeconds(explosionDelay);

        if (explosionPrefab != null)
        {
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        }
        else
        {
            Debug.LogWarning("DetonabotAI: Explosion prefab not assigned.");
        }

        Destroy(gameObject); 
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
