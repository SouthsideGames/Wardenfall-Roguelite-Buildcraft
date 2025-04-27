using UnityEngine;

/// <summary>
/// Represents a healing zone that restores health to entities within it
/// </summary>
public class HealZone : EnvironmentalHazard
{
    [SerializeField] private int healAmount = 5;    // Amount of health restored
    [SerializeField] private Color healColor = Color.green;  // Visual color for the zone
    
    private SpriteRenderer spriteRenderer;

    protected override void Start()
    {
        base.Start();
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            Color color = healColor;
            color.a = 0.3f; // Make it semi-transparent
            spriteRenderer.color = color;
        }
    }

    protected override void ApplyHazardEffect(Collider2D other)
    {
        // For player healing
        if (other.CompareTag("Player"))
        {
            CharacterManager.Instance.health.Heal(healAmount);
        }
        
        // For enemy healing
        Enemy enemy = other.GetComponent<Enemy>();
        if (enemy != null)
        {
            enemy.Heal(healAmount);
        }
    }
}
