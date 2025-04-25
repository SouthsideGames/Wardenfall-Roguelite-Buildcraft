using UnityEngine;

public class FireballEffect : MonoBehaviour, ICardEffect
{
    [SerializeField] private GameObject fireballPrefab;
    [SerializeField] private float speed = 10f;

    public void Activate(CharacterManager target, CardSO card)
    {
        if (fireballPrefab == null || target == null)
        {
            return;
        }

        Vector2 randomDirection = Random.insideUnitCircle.normalized;

        GameObject fireball = Instantiate(fireballPrefab, target.transform.position, Quaternion.identity);

        Rigidbody2D rb = fireball.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = randomDirection * speed;
        }

        FireballProjectile projectile = fireball.GetComponent<FireballProjectile>();
        if (projectile != null)
        {
            projectile.Initialize((int)card.effectValue);
        }

        Destroy(gameObject);
    }

    public void Deactivate() { }
    public void Tick(float deltaTime) { }
}
