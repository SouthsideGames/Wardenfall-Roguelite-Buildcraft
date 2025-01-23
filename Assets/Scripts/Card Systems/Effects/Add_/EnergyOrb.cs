using UnityEngine;

public class EnergyOrb : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f; // Orb speed
    [SerializeField] private float damage = 50;     // Damage dealt
    [SerializeField] private float explosionRadius = 2f; // Explosion radius
    [SerializeField] private ParticleSystem explosionEffect; // Visual effect on impact
    [SerializeField] private LayerMask enemyMask; // Targets affected by the explosion

    private Vector2 direction;

    public void Launch(Vector2 launchDirection, CardSO _card)
    {
        direction = launchDirection.normalized;
        damage = _card.EffectValue;
    }

    private void Update()
    {
        transform.Translate(direction * moveSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (((1 << collision.gameObject.layer) & enemyMask) != 0)
        {
            Explode();
        }
    }

    private void Explode()
    {
        if (explosionEffect != null)
            Instantiate(explosionEffect, transform.position, Quaternion.identity);

        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, explosionRadius, enemyMask);
        foreach (var enemy in hitEnemies)
        {
            enemy.GetComponent<Enemy>()?.TakeDamage((int)damage, false);
        }

        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
