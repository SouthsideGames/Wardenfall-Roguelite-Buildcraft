using UnityEngine;


public class PoisonCloudZone : MonoBehaviour
{
    [SerializeField] private float tickInterval = 1f;
    [SerializeField] private LayerMask enemyMask;
    [SerializeField] private float radius = 2f;

    private int damage;
    private float lifetime;
    private float timer;

    public void Initialize(int damagePerTick, float duration)
    {
        damage = damagePerTick;
        lifetime = duration;
        timer = tickInterval;
        Destroy(gameObject, lifetime);

    }

    private void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            ApplyDamage();
            timer = tickInterval;
        }
    }

    private void ApplyDamage()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, radius, enemyMask);
        foreach (Collider2D hit in hits)
        {
            Enemy enemy = hit.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage, false);
            }
        }
    }
}