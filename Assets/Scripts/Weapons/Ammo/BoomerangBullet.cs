using UnityEngine;

public class BoomerangBullet : BulletBase
{
    private Vector2 startPosition;
    private Vector2 returnTarget;
    private float maxDistance;
    private float returnSpeedMultiplier;
    private bool isReturning;

    public void Shoot(int _damage, Vector2 direction, bool _isCriticalHit, float _maxDistance, float _returnSpeedMultiplier)
    {
        base.Shoot(_damage, direction, _isCriticalHit);

        startPosition = transform.position;
        maxDistance = _maxDistance;
        returnSpeedMultiplier = _returnSpeedMultiplier;
        isReturning = false;
    }

    private void Update()
    {
        if (!isReturning && Vector2.Distance(startPosition, transform.position) >= maxDistance)
        {
            StartReturn();
        }

        if (isReturning)
        {
            ReturnToPlayer();
        }
    }

    private void StartReturn()
    {
        isReturning = true;
        returnTarget = rangedWeapon.transform.position;
    }

    private void ReturnToPlayer()
    {
        Vector2 direction = (returnTarget - (Vector2)transform.position).normalized;
        rb.linearVelocity = direction * moveSpeed * returnSpeedMultiplier;

        if (Vector2.Distance(transform.position, returnTarget) <= 0.5f)
        {
            DestroyBullet();
        }
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            Enemy enemy = collision.GetComponent<Enemy>();
            if (enemy != null)
            {
                ApplyDamage(enemy);

            }
        }
    }
}
