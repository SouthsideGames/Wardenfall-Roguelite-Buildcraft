using UnityEngine;
using System;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class EnemyBullet : MonoBehaviour
{

    [Header("ELEMENTS:")]
    protected Rigidbody2D rb;
    private Collider2D col;
    private RangedEnemyAttack rangedEnemyAttack;

    [Header("SETTINGS:")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float angularSpeed;
    protected int damage;

    // Added flag to prevent multiple releases
    private bool isReleased = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
    }

    // Configure the bullet and reset release flag
    public void Configure(RangedEnemyAttack _rangedEnemyAttack)
    {
        rangedEnemyAttack = _rangedEnemyAttack;
        isReleased = false; // Reset the flag when reused
    }

    // Shoot logic
    public void Shoot(int _damage, Vector2 _direction)
    {
        damage = _damage;

        if (Mathf.Abs(_direction.x + 1) < 0.01f)
            _direction.y += .01f;

        transform.right = _direction;
        rb.linearVelocity = _direction * moveSpeed;
        rb.angularVelocity = angularSpeed;

        // Schedule release with LeanTween and check flag
        LeanTween.cancel(gameObject); // Ensure no lingering tweens
        LeanTween.delayedCall(gameObject, 5, () => ReleaseBullet());
    }

    // Handle collisions
    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.TryGetComponent(out CharacterManager player))
        {
            player.TakeDamage(damage);
            col.enabled = false;
            ReleaseBullet(); // Use new method to check flag
        }
        else if (collider.TryGetComponent(out SurvivorBox box))
        {
            box.TakeDamage(damage);
            col.enabled = false;
            ReleaseBullet(); // Use new method to check flag
        }
    }

    // Reset bullet properties when reused
    public void Reload()
    {
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0;
        col.enabled = true;

        LeanTween.cancel(gameObject);

        // Schedule delayed release with the flag check
        LeanTween.delayedCall(gameObject, 5, () => ReleaseBullet());

        // Reset release flag
        isReleased = false;
    }

    // New method to handle safe release
    protected void ReleaseBullet()
    {
        if (!isReleased) // Prevent multiple releases
        {
            isReleased = true; // Mark as released
            rangedEnemyAttack.ReleaseBullet(this); // Return to pool
        }
    }
}
