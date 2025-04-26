using UnityEngine;

/// <summary>
/// Creates a zone that periodically strikes entities with lightning
/// </summary>
public class LightningHazard : EnvironmentalHazard
{
    [SerializeField] private float strikeInterval = 3f;
    [SerializeField] private Color warningColor = Color.yellow;
    [SerializeField] private Color normalColor = new Color(0.8f, 0.8f, 0.2f, 0.2f);
    
    private SpriteRenderer spriteRenderer;
    private float nextStrikeTime;
    private bool isWarning;

    protected override void Start()
    {
        base.Start();
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.color = normalColor;
        }
        nextStrikeTime = Time.time + strikeInterval;
    }

    private void Update()
    {
        if (Time.time >= nextStrikeTime - 0.5f && !isWarning)
        {
            isWarning = true;
            spriteRenderer.color = warningColor;
        }
        
        if (Time.time >= nextStrikeTime)
        {
            Strike();
            nextStrikeTime = Time.time + strikeInterval;
            isWarning = false;
            spriteRenderer.color = normalColor;
        }
    }

    private void Strike()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, transform.localScale.x / 2f);
        foreach (Collider2D hit in hits)
        {
            if (hit.CompareTag("Player") || hit.CompareTag("Enemy"))
            {
                ApplyHazardEffect(hit);
            }
        }
    }
}