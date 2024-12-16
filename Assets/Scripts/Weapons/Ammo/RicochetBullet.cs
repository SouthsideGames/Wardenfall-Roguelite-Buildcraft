using UnityEngine;

public class RicochetBullet : BulletBase
{
    [Header("Ricochet Settings")]
    [SerializeField] private int maxBounces = 3; // Maximum number of bounces.
    private int remainingBounces;

    public override void Shoot(int _damage, Vector2 _direction, bool _isCriticalHit)
    {
        base.Shoot(_damage, _direction, _isCriticalHit);
        remainingBounces = maxBounces; // Reset bounce count when the bullet is fired.
    }

    protected override void OnTriggerEnter2D(Collider2D collider)
    {
        if (IsInLayerMask(collider.gameObject.layer, enemyMask))
        {
            Enemy enemy = collider.GetComponent<Enemy>();
            if (enemy != null)
            {
                ApplyDamage(enemy); // Apply damage to the enemy.
            }

            Bounce();
        }
        else
        {
            // Bounce off walls or other objects.
            Bounce();
        }
    }

    private void Bounce()
    {
        if (remainingBounces > 0)
        {
            remainingBounces--;

            // Reflect the bullet's velocity off the surface it collided with.
            Vector2 reflection = Vector2.Reflect(rb.linearVelocity.normalized, GetCollisionNormal());
            rb.linearVelocity = reflection * moveSpeed;
        }
        else
        {
            // If no bounces are left, release the bullet.
            Release();
        }
    }

    private Vector2 GetCollisionNormal()
    {
        // Get the normal vector of the surface the bullet collided with.
        RaycastHit2D hit = Physics2D.Raycast(transform.position, rb.linearVelocity.normalized, 0.1f, ~enemyMask);
        return hit.normal;
    }
}
