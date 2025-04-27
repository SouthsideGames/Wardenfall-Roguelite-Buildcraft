using UnityEngine;

public class HealZone : EnvironmentalHazard
{
    [SerializeField] private int healAmount = 5;
    [SerializeField] private Color healColor = Color.green; 
    
    private SpriteRenderer spriteRenderer;

    protected override void Start()
    {
        base.Start();
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            Color color = healColor;
            color.a = 0.3f; 
            spriteRenderer.color = color;
        }
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
