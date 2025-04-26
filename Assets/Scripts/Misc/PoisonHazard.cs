using UnityEngine;

/// <summary>
/// Represents a poison hazard that applies both damage and a poison status effect
/// Extends base hazard by adding poison damage over time
/// </summary>
public class PoisonHazard : EnvironmentalHazard
{
    [SerializeField] private float poisonDuration = 3f;    // How long poison effect lasts
    
    /// <summary>
    /// Applies both direct damage and poison status effect to the player
    /// </summary>
    protected override void ApplyHazardEffect(Collider2D other)
    {
        // Apply base damage first
        base.ApplyHazardEffect(other);
        
        // Apply additional poison status effect
        EnemyStatus playerStatus = other.GetComponent<EnemyStatus>();
        if (playerStatus != null)
        {
            StatusEffect poisonEffect = new StatusEffect(StatusEffectType.Poison, poisonDuration);
            playerStatus.ApplyEffect(poisonEffect);
        }
    }
}
