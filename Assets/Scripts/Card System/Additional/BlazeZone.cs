using System.Collections;
using UnityEngine;

public class BladeZone : MonoBehaviour
{
    [SerializeField] private float tickInterval = 1f;
    [SerializeField] private float radius = 2f;
    [SerializeField] private LayerMask enemyMask;
    [SerializeField] private Transform spinTransform;
    [SerializeField] private float spinSpeed = 360f; // degrees per second

    private float damage;
    private float duration;
    private float timer;

    public void Initialize(float damagePerTick, float durationSeconds)
    {
        damage = damagePerTick;
        duration = durationSeconds;
        timer = 0f;
        StartCoroutine(DestroyAfterDuration());
    }

    private void Update()
    {
        // Rotate the blades
        if (spinTransform != null)
        {
            spinTransform.Rotate(Vector3.forward, spinSpeed * Time.deltaTime);
        }

        // Tick damage
        timer += Time.deltaTime;
        if (timer >= tickInterval)
        {
            timer = 0f;
            ApplyDamage();
        }
    }

    private void ApplyDamage()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, radius, enemyMask);
        foreach (Collider2D hit in hits)
        {
            Enemy enemy = hit.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage((int)damage);
            }
        }
    }

    private IEnumerator DestroyAfterDuration()
    {
        yield return new WaitForSeconds(duration);
        Destroy(gameObject);
    }
}
