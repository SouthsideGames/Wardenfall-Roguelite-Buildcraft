using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplodeEnemy : Enemy
{
    [Header("Exploder Specific")]
    [SerializeField] private float explosionRadius = 3f; // Radius of the explosion
    [SerializeField] private int explosionDamage = 10; // Damage dealt by the explosion
    [SerializeField] private ParticleSystem explosionEffect; // Particle effect for explosion
    [SerializeField] private float explosionTriggerDistance = 1.5f; // Distance at which the enemy will explode

    private bool isExploding = false; // Flag to prevent multiple explosions

    protected override void Start()
    {
        base.Start();
        // Additional initialization if needed
    }

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
        return Vector2.Distance(transform.position, player.transform.position) <= explosionTriggerDistance;
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
                enemy.TakeDamage(explosionDamage);
            }

            // Damage the player if within range
            if (hit.TryGetComponent<PlayerManager>(out PlayerManager player))
            {
                player.TakeDamage(explosionDamage);
            }
        }

        Debug.Log($"{gameObject.name} exploded.");

        // Destroy the enemy after exploding
        Die();
    }

    private void OnDrawGizmosSelected()
    {
        // Draw explosion radius and trigger distance for visualization in the editor
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionTriggerDistance);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
