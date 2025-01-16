using UnityEngine;

public class Blade : MonoBehaviour
{
     [Tooltip("Damage dealt by the blade.")]
    [SerializeField] private int damage = 10;

    [Tooltip("Lifetime of the blade in seconds.")]
    [SerializeField] private float lifetime = 2f;
    private float rotationSpeed = 360f;

    [Tooltip("Detection layer for enemies.")]
    [SerializeField] private LayerMask enemyMask;

    private Collider2D[] colliders;

    private void Start()
    {

        colliders = GetComponentsInChildren<Collider2D>();

        if (colliders.Length == 0)
        {
            Debug.LogWarning("No colliders found on Blade or its children.");
        }

        Destroy(gameObject, lifetime);
    }

    private void Update()
    {
        transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        ProcessCollision(collision);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        ProcessCollision(collision.collider);
    }

    private void ProcessCollision(Collider2D collider)
    {
        if (((1 << collider.gameObject.layer) & enemyMask) != 0)
        {
            Enemy enemy = collider.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage, false);
            }
        }
    }
}
