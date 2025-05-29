using UnityEngine;

public class Doomnut : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 8f;
    [SerializeField] private int contactDamage = 20;
    [SerializeField] private float bounceForce = 6f;
    [SerializeField] private GameObject impactVFX;

    [SerializeField] private AudioClip spawnSound;
    [SerializeField] private AudioClip impactSound;

    private Rigidbody2D rb;

    void Awake() => rb = GetComponent<Rigidbody2D>();


    public void Initialize(float duration)
    {
        Launch();
        Destroy(gameObject, duration);

        if (spawnSound != null)
            AudioManager.Instance.PlaySFX(spawnSound);
    }

    private void Launch()
    {
        Vector2 randomDir = Random.insideUnitCircle.normalized;
        rb.linearVelocity = randomDir * moveSpeed;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Enemy enemy = collision.collider.GetComponent<Enemy>();
        if (enemy != null)
        {
            enemy.TakeDamage(contactDamage);
            if (impactVFX != null)
                Instantiate(impactVFX, transform.position, Quaternion.identity);
        }

        Vector2 reflect = Vector2.Reflect(rb.linearVelocity, collision.contacts[0].normal);
        rb.linearVelocity = reflect.normalized * bounceForce;

        if (impactSound != null)
            AudioManager.Instance.PlaySFX(impactSound);
    }
}
