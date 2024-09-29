using UnityEngine;

public class ExplodeEnemy : Enemy
{
    [Header("EXPLODER SPECIFICS:")]
    [SerializeField] private float explosionRadius = 3f; // Radius of the explosion
    [SerializeField] private int explosionDamage = 10; // Damage dealt by the explosion
    [SerializeField] private ParticleSystem explosionEffect; // Particle effect for explosion
    private bool isExploding = false; // Flag to prevent multiple explosions

    void Update()
    {
        if (!hasSpawned || !CanAttack() || isExploding)
            return;

        // Check if it's close enough to explode
        if (IsPlayerTooClose())
        {
            Explode();
        }
        else
        {
            // Move towards the player
            movement.FollowPlayer();
        }
    }

    

    private bool IsPlayerTooClose()
    {
        // Check if the player is within the trigger distance for explosion
        return Vector2.Distance(transform.position, character.transform.position) <= playerDetectionRadius;
    }

    private void Explode()
    {
        if (isExploding) return; // Prevent multiple explosions
        isExploding = true;

        // Play explosion effect
        if (explosionEffect != null)
        {
            ParticleSystem effect = Instantiate(explosionEffect, transform.position, Quaternion.identity);
            effect.Play();
        }

        // Find all objects within explosion radius
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, explosionRadius);

        foreach (Collider2D hit in hits)
        {
            // Check if the hit object can take damage
            if (hit.TryGetComponent<Enemy>(out Enemy enemy) && enemy != this)
            {
                enemy.TakeDamage(explosionDamage, false);
            }

            // Damage the player if within range
            if (hit.TryGetComponent<CharacterManager>(out CharacterManager player))
            {
                player.TakeDamage(explosionDamage);
            }
        }

        // Destroy the enemy after exploding
        Die();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
