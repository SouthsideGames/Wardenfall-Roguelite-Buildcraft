using UnityEngine;

public class Thunderbolt : MonoBehaviour
{
    [Tooltip("The radius of the AoE damage.")]
    [SerializeField] private float explosionRadius = 3f;

    [Tooltip("The particle effect for impact.")]
    [SerializeField] private ParticleSystem impactEffect;

    [Tooltip("LayerMask for detecting enemies.")]
    [SerializeField] private LayerMask enemyMask;

    public void Strike(Vector2 targetPosition, CardSO _card)
    {
        transform.position = targetPosition;

        if (impactEffect != null)
            Instantiate(impactEffect, targetPosition, Quaternion.identity);

        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(targetPosition, explosionRadius, enemyMask);

        foreach (var enemy in hitEnemies)
        {
            enemy.GetComponent<Enemy>()?.TakeDamage((int)_card.EffectValue, false);
        }

        Destroy(gameObject, 0.5f);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
