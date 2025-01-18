using UnityEngine;

public class Fireball : MonoBehaviour
{
     [Header("FIREBALL SETTINGS")]
    [SerializeField] private float moveSpeed = 8f;
    [SerializeField] private float lifetime = 3f;
    [SerializeField] private LayerMask enemyMask;
    [SerializeField] private GameObject explosionPrefab; // Optional explosion effect

    private Rigidbody2D rb;
    private int damage;
    private Vector2 direction;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Launch(int _damage, Vector2 _direction)
    {
        damage = _damage;
        direction = _direction.normalized;

        rb.linearVelocity = direction * moveSpeed;
        Invoke(nameof(DestroyFireball), lifetime);
    }

    private void OnParticleCollision(GameObject collision) 
    {
         if (IsInLayerMask(collision.gameObject.layer, enemyMask))
        {
            Enemy enemy = collision.GetComponent<Enemy>();
            if (enemy != null)
            {
                ApplyDamage(enemy);
                Explode();
            }
        }
    }

    private void ApplyDamage(Enemy enemy)
    {
        enemy.TakeDamage(damage, false);
    }

    /// <summary>
    /// Handles the explosion logic, including creating a visual effect and destroying the fireball.
    /// </summary>
    private void Explode()
    {
        if (explosionPrefab != null)
        {
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        }

        DestroyFireball();
    }

    /// <summary>
    /// Destroys the fireball and stops its movement.
    /// </summary>
    private void DestroyFireball()
    {
        rb.linearVelocity = Vector2.zero;
        gameObject.SetActive(false);
    }

    private bool IsInLayerMask(int layer, LayerMask layerMask)
    {
        return (layerMask.value & (1 << layer)) != 0;
    }
}
