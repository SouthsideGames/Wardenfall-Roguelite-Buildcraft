using UnityEngine;

public class LightningHazard : EnvironmentalHazard
{
    [SerializeField] private float strikeInterval = 3f;
     private float nextStrikeTime;
    private bool isWarning;

    protected override void Start()
    {
        base.Start();
    
        nextStrikeTime = Time.time + strikeInterval;
    }

    private void Update()
    {
        if (Time.time >= nextStrikeTime - 0.5f && !isWarning)
            isWarning = true;
        
        if (Time.time >= nextStrikeTime)
        {
            Strike();
            nextStrikeTime = Time.time + strikeInterval;
            isWarning = false;
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