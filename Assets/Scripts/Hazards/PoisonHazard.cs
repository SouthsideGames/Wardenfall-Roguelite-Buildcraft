using UnityEngine;

public class PoisonHazard : EnvironmentalHazard
{
    [SerializeField] private float poisonDuration = 3f;
    
    protected override void ApplyHazardEffect(Collider2D other)
    {
        base.ApplyHazardEffect(other);
        
        EnemyStatus enemy = other.GetComponent<EnemyStatus>();
        if (enemy != null)
        {
            StatusEffect poisonEffect = new StatusEffect(StatusEffectType.Poison, poisonDuration);
            enemy.ApplyEffect(poisonEffect);
        }
    }
}
