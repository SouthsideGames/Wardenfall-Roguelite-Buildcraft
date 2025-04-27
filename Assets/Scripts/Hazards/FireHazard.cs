using UnityEngine;

public class FireHazard : EnvironmentalHazard 
{
    [SerializeField] private float burnDuration = 2f; 

    protected override void ApplyHazardEffect(Collider2D other)
    {
        base.ApplyHazardEffect(other);
        
        EnemyStatus enemyStatus = other.GetComponent<EnemyStatus>();

        if (enemyStatus != null)
        {
            StatusEffect burnEffect = new StatusEffect(StatusEffectType.Burn, burnDuration);
            enemyStatus.ApplyEffect(burnEffect);
        }

    }
}
