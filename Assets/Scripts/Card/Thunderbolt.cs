using UnityEngine;

public class Thunderbolt : MonoBehaviour
{
    [Tooltip("The radius of the AoE damage.")]
    [SerializeField] private float explosionRadius = 3f;

    [Tooltip("The amount of damage dealt.")]
    [SerializeField] private int damage = 100;

    [Tooltip("The particle effect for impact.")]
    [SerializeField] private ParticleSystem impactEffect;

    [Tooltip("LayerMask for detecting enemies.")]
    [SerializeField] private LayerMask enemyMask;

    public void Strike(Vector2 targetPosition, int damageMultiplier)
    {
        transform.position = targetPosition;

        if (impactEffect != null)
            Instantiate(impactEffect, targetPosition, Quaternion.identity);

        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(targetPosition, explosionRadius, enemyMask);

        foreach (var enemy in hitEnemies)
        {
            enemy.GetComponent<Enemy>()?.TakeDamage(damage * damageMultiplier / 100, false);
        }

        Destroy(gameObject, 0.5f);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
