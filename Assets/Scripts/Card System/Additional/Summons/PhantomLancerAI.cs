using UnityEngine;
using System.Collections;

public class PhantomLancerAI : MonoBehaviour
{
    [SerializeField] private float dashSpeed = 10f;
    [SerializeField] private float dashCooldown = 0.4f;
    [SerializeField] private float dashRange = 8f;
    [SerializeField] private int dashDamage = 80;

    private float lifetime;
    private float lifetimeRemaining;

    public void Initialize(float duration)
    {
        lifetime = duration;
        lifetimeRemaining = duration;
        StartCoroutine(AttackLoop());
    }

    void Update()
    {
        lifetimeRemaining -= Time.deltaTime;
        if (lifetimeRemaining <= 0f)
            Destroy(gameObject);
    }

    private IEnumerator AttackLoop()
    {
        while (lifetimeRemaining > 0f)
        {
            Enemy target = FindNearestEnemy();
            if (target != null)
            {
                transform.position = target.transform.position + (Vector3)(Random.insideUnitCircle * 0.2f);
                yield return new WaitForSeconds(0.05f); // Flash effect or animation delay

                target.TakeDamage(dashDamage); // Direct damage
            }

            yield return new WaitForSeconds(dashCooldown);
        }
    }

    private Enemy FindNearestEnemy()
    {
        Enemy[] enemies = FindObjectsByType<Enemy>(FindObjectsSortMode.None);
        Enemy closest = null;
        float minDist = dashRange;

        foreach (var enemy in enemies)
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
