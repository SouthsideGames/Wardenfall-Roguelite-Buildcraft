using UnityEngine;

public class Fireball : MonoBehaviour
{
     [Header("FIREBALL SETTINGS")]
    [SerializeField] private float moveSpeed = 8f;
    [SerializeField] private float lifetime = 3f;
    [SerializeField] private LayerMask enemyMask;
    [SerializeField] private GameObject explosionPrefab; // Optional explosion effect
    [SerializeField] private int baseDamage = 10;

    private Rigidbody2D rb;
    private int damage;
    private bool isCriticalHit;
    private Vector2 direction;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    /// <summary>
    /// Launches the fireball in a specific direction with given damage and critical hit status.
    /// </summary>
    /// <param name="_damage">The damage the fireball will deal.</param>
    /// <param name="_direction">The direction to launch the fireball.</param>
    /// <param name="_isCriticalHit">Whether this is a critical hit.</param>
    public void Launch(int _damage, Vector2 _direction, bool _isCriticalHit)
    {
        damage = _damage;
        direction = _direction.normalized;
        isCriticalHit = _isCriticalHit;

        rb.linearVelocity = direction * moveSpeed;
        Invoke(nameof(DestroyFireball), lifetime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
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

    /// <summary>
    /// Applies damage to the target enemy.
    /// </summary>
    /// <param name="enemy">The enemy to apply damage to.</param>
    private void ApplyDamage(Enemy enemy)
    {
        enemy.TakeDamage(damage, isCriticalHit);
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
