using UnityEngine;

public class HomingBullet : EnemyBullet
{
     [Header("Homing Spike Settings")]
    [SerializeField] private float pauseDuration = 0.5f;
    [SerializeField] private float homingSpeed = 12f;
    
    private Vector2 targetPosition;
    private bool isLockedOn = false;
    private bool hasFired = false;

    public void Initialize(Vector2 playerPosition)
    {
        targetPosition = playerPosition;
        isLockedOn = false;
        hasFired = false;

        rb.linearVelocity = Vector2.zero; // Stop movement during pause
        Invoke(nameof(LockOnAndFire), pauseDuration);
    }

    private void LockOnAndFire()
    {
        isLockedOn = true;
        hasFired = true;
    }

    private void Update()
    {
        if (isLockedOn && hasFired)
        {
            rb.linearVelocity = (targetPosition - (Vector2)transform.position).normalized * homingSpeed;
            
            if (Vector2.Distance(transform.position, targetPosition) < 0.1f)
            {
                ReleaseBullet(); // Return to pool instead of destroying
            }
        }
    }

    // Handle collisions (inherits from EnemyBullet)
    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Player"))
        {
            collider.GetComponent<CharacterManager>().TakeDamage(damage);
            ReleaseBullet();
        }
    }
}
