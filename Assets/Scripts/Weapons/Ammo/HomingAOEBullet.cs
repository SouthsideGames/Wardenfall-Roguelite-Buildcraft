using System.Collections;
using UnityEngine;

// TODO: Add explosion effect and trail rend
public class HomingAOEBullet : BulletBase
{
    public float explosionRadius;
    [SerializeField] private float waveFrequency = 2f; // Frequency of the wave
    [SerializeField] private float waveAmplitude = 0.5f; // Amplitude of the wave
    private float waveTimer = 0f;
    private Vector2 moveDirection;

    public void Initialize(int _damage, Vector2 direction, bool _isCriticalHit, float _explosionRadius, LayerMask _enemyMask)
    {
        base.Shoot(_damage, direction, _isCriticalHit);
        explosionRadius = _explosionRadius;
        enemyMask = _enemyMask;
        moveDirection = direction.normalized;
        UpdateTarget();
    }

    private void Update()
    {
        if (target == null)
        {
            MoveInWave();
        }
        else
        {
            UpdateTarget(); // Update target if it's moving
        }
    }

    private void MoveInWave()
    {
        waveTimer += Time.deltaTime;
        Vector2 waveOffset = new Vector2(0, Mathf.Sin(waveTimer * waveFrequency) * waveAmplitude);
        Vector2 newPosition = (Vector2)transform.position + (moveDirection * moveSpeed * Time.deltaTime) + waveOffset;
        rb.MovePosition(newPosition);
    }

    private void UpdateTarget()
    {
        Enemy closestEnemy = GetClosestTarget();

        if (closestEnemy != null)
        {
            if (target != null && target != closestEnemy)
            {
                target.DeactivateLockedOnIndicator();
            }

            target = closestEnemy;
            target.ActivateLockedOnIndicator();
        }
        else if (target != null)
        {
            target.DeactivateLockedOnIndicator();
            target = null;
        }
    }

    private Enemy GetClosestTarget()
    {
        Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, explosionRadius, enemyMask);
        Enemy closest = null;
        float closestDistance = Mathf.Infinity;

        foreach (Collider2D enemyCollider in enemies)
        {
            Enemy enemy = enemyCollider.GetComponent<Enemy>();
            if (enemy != null)
            {
                float distance = Vector2.Distance(transform.position, enemy.transform.position);
                if (distance < closestDistance)
                {
                    closest = enemy;
                    closestDistance = distance;
                }
            }
        }
        return closest;
    }

    protected override void OnTriggerEnter2D(Collider2D collider)
    {
        if (IsInLayerMask(collider.gameObject.layer, enemyMask))
        {
            Explode();
        }
    }

    private void Explode()
    {
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, explosionRadius, enemyMask);

        foreach (Collider2D enemy in hitEnemies)
        {
            Enemy hitEnemy = enemy.GetComponent<Enemy>();
            if (hitEnemy != null)
            {
                ApplyDamage(hitEnemy);
            }
        }

        if (target != null)
        {
            target.DeactivateLockedOnIndicator();
        }

        DestroyBullet();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
