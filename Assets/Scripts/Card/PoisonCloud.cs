using UnityEngine;

public class PoisonCloud : MonoBehaviour
{

    [Tooltip("Chance to apply poison effect (0 to 1).")]
    [SerializeField, Range(0f, 1f)] private float poisonChance;

    [Tooltip("Poison duration on enemies.")]
    [SerializeField] private float poisonDuration;

    [Tooltip("Poison damage per second.")]
    [SerializeField] private int poisonDamagePerSecond;

    [Tooltip("Detection layer for enemies.")]
    [SerializeField] private LayerMask enemyMask;

    private float timer;
    private float duration;
    private int damage;


    public void Configure(int damagePerSecond, float duration, float poisonChance, float poisonDuration, int poisonDamagePerSecond)
    {
        this.damage = damagePerSecond;
        this.duration = duration;
        this.poisonChance = poisonChance;
        this.poisonDuration = poisonDuration;
        this.poisonDamagePerSecond = poisonDamagePerSecond;
    }

    private void Start()
    {
        timer = duration;
        Destroy(gameObject, duration);
    }

    private void Update()
    {
        timer -= Time.deltaTime;
    }

    private void OnParticleCollision(GameObject collision)
    {
        if (((1 << collision.gameObject.layer) & enemyMask) != 0)
        {
            Enemy enemy = collision.GetComponent<Enemy>();
            if (enemy != null)
            {
                // Apply direct damage
                enemy.TakeDamage(damage, false);

                // Check if poison should be applied
                if (Random.value <= poisonChance)
                {
                    ApplyPoisonEffect(enemy);
                }
            }
        }
    }

    private void ApplyPoisonEffect(Enemy enemy)
    {
        StatusEffect poisonEffect = new StatusEffect(
            StatusEffectType.Poison,
            poisonDuration,
            poisonDamagePerSecond,
            interval: 1f
        );

        enemy.GetComponent<EnemyStatus>()?.ApplyEffect(poisonEffect);
    }

}
