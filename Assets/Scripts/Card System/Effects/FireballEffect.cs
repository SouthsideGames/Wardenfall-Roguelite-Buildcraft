using UnityEngine;

public class FireballEffect : MonoBehaviour, ICardEffect
{
    [SerializeField] private GameObject fireballPrefab;
    [SerializeField] private float speed = 10f;

    public void Activate(CharacterManager target, CardSO card)
    {
        if (fireballPrefab == null || target == null)
        {
            Debug.LogWarning("FireballEffect missing reference.");
            return;
        }

        // Choose a random 2D direction
        Vector2 randomDirection = Random.insideUnitCircle.normalized;

        // Spawn fireball at the player position
        GameObject fireball = Instantiate(fireballPrefab, target.transform.position, Quaternion.identity);

        // Apply velocity
        Rigidbody2D rb = fireball.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = randomDirection * speed;
        }

        // Pass damage and lifetime to projectile
        FireballProjectile projectile = fireball.GetComponent<FireballProjectile>();
        if (projectile != null)
        {
            projectile.Initialize((int)card.effectValue);
        }

        // Auto-destroy effect wrapper
        Destroy(gameObject);
    }

    public void Deactivate() { }
    public void Tick(float deltaTime) { }
}
