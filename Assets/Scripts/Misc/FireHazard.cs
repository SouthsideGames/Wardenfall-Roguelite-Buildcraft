using UnityEngine;

/// <summary>
/// Represents a fire hazard that applies both damage and a burn status effect
/// Includes visual particle effects for fire
/// </summary>
public class FireHazard : EnvironmentalHazard 
{
    [SerializeField] private ParticleSystem fireParticles;  // Visual fire effect
    [SerializeField] private float burnDuration = 2f;       // How long burn effect lasts

    /// <summary>
    /// Applies both direct damage and burn status effect to the player
    /// </summary>
    protected override void ApplyHazardEffect(Collider2D other)
    {
        // Apply base damage first
        base.ApplyHazardEffect(other);
        
        // Apply additional burn status effect
        EnemyStatus playerStatus = other.GetComponent<EnemyStatus>();
        if (playerStatus != null)
        {
            StatusEffect burnEffect = new StatusEffect(StatusEffectType.Burn, burnDuration);
            playerStatus.ApplyEffect(burnEffect);
        }
    }
}
