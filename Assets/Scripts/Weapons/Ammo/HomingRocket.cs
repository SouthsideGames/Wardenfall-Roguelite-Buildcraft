using System.Collections;
using UnityEngine;

// TODO: Add explosion effect and trail rend
public class HomingRocket : BulletBase
{

    [Header("EXPLOSION SETTINGS:")]
    [SerializeField] private GameObject explosionPrefab;
    [SerializeField] private AudioClip explosionSoundEffect;

    private float explosionRadius;
    private Vector2 moveDirection;
    private AudioSource audioSource;

    protected override void Awake() 
    {
        base.Awake();

        audioSource = gameObject.AddComponent<AudioSource>();
    }

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
            UpdateTarget();
    }


    private void UpdateTarget()
    {
        Enemy closestEnemy = GetClosestTarget();

        if (closestEnemy != null)
        {

            target = closestEnemy;
        }
        else if (target != null)
        {

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

        PlaySFX();
        Instantiate(explosionPrefab, transform.position, Quaternion.identity);    

        DestroyBullet();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }

    
    protected void PlaySFX()
    {
        audioSource.clip = explosionSoundEffect;
        audioSource.pitch = Random.Range(0.95f, 1.05f);
        audioSource.Play();
    }

}
