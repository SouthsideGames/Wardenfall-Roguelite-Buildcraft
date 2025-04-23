using UnityEngine;

public class FireballProjectile : MonoBehaviour
{
    [SerializeField] private float lifeTime = 5f;

    private int damage;

    public void Initialize(int damageValue)
    {
        damage = damageValue;
        Destroy(gameObject, lifeTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage, false);
            }
        }

        Destroy(gameObject);
    }
}
