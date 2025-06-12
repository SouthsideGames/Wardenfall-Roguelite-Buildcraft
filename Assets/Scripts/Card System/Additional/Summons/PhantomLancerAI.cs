using UnityEngine;
using System.Collections;

public class PhantomLancerAI : MonoBehaviour
{
    [SerializeField] private float dashSpeed = 10f;
    [SerializeField] private float dashCooldown = 0.4f;
    [SerializeField] private float dashRange = 8f;
    [SerializeField] private int dashDamage = 80;
    [SerializeField] private float attackDistance = 0.5f;

    private float lifetime;
    private float lifetimeRemaining;
    private bool isDashing = false;

    public void Initialize(float duration)
    {
        lifetime = duration;
        lifetimeRemaining = duration;
        StartCoroutine(AttackLoop());
    }

    void Update()
    {
        lifetimeRemaining -= Time.deltaTime;
        if (lifetimeRemaining <= 0f && !isDashing)
        {
            Destroy(gameObject);
        }
    }

    private IEnumerator AttackLoop()
    {
        while (lifetimeRemaining > 0f)
        {
            Enemy target = FindNearestEnemy();
            if (target != null)
            {
                isDashing = true;
                yield return StartCoroutine(DashToTarget(target));
                target.TakeDamage(dashDamage);
                isDashing = false;
            }

            yield return new WaitForSeconds(dashCooldown);
        }
    }

    private IEnumerator DashToTarget(Enemy target)
    {
        if (target == null)
            yield break;

        Vector3 start = transform.position;
        Vector3 dir = (target.transform.position - start).normalized;
        Vector3 end = target.transform.position - (Vector3)(dir * attackDistance);

        float distance = Vector3.Distance(start, end);
        float duration = distance / dashSpeed;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            if (target == null) yield break;

            elapsed += Time.deltaTime;
            transform.position = Vector3.Lerp(start, end, elapsed / duration);
            yield return null;
        }

        transform.position = end + (Vector3)(Random.insideUnitCircle * 0.1f); // subtle offset
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
