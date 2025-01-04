using UnityEngine;

public class RicochetBullet : BulletBase
{
    [Header("Ricochet Settings")]
    [SerializeField] private int maxBounces = 3; // Maximum number of bounces.
    [SerializeField] private AudioClip bounceSoundEffect;

    private int remainingBounces;
    private AudioSource audioSource;


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
                ApplyDamage(enemy); 

            Bounce();
        }
        else
            Bounce();
    }

    private void Bounce()
    {
        if (remainingBounces > 0)
        {
            PlaySFX();

            remainingBounces--;

            Vector2 reflection = Vector2.Reflect(rb.linearVelocity.normalized, GetCollisionNormal());
            rb.linearVelocity = reflection * moveSpeed;
        }
        else
            Release();
    }

    private Vector2 GetCollisionNormal()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, rb.linearVelocity.normalized, 0.1f, ~enemyMask);
        return hit.normal;
    }

     protected void PlaySFX()
    {
        audioSource.clip = bounceSoundEffect;
        audioSource.pitch = Random.Range(0.95f, 1.05f);
        audioSource.Play();
    }
}
