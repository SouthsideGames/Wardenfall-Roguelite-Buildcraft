using UnityEngine;

public class HealZone : EnvironmentalHazard
{
    [SerializeField] private int healAmount = 5;

    protected override void Start()
    {
        base.Start();
    }

    protected override void ApplyHazardEffect(Collider2D other)
    {
        // For player healing
        if (other.CompareTag("Player"))
            CharacterManager.Instance.health.Heal(healAmount);

        Enemy enemy = other.GetComponent<Enemy>();
        if (enemy != null)
        {
            enemy.Heal(healAmount);
        }
    }
}
