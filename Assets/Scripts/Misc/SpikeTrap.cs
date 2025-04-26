using UnityEngine;

/// <summary>
/// Represents a spike trap hazard that deals instant damage to the player
/// Unlike other hazards, deals damage immediately on each collision without intervals
/// </summary>
public class SpikeTrap : EnvironmentalHazard
{
    /// <summary>
    /// Overrides base behavior to apply damage instantly without intervals
    /// </summary>
    protected override void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            ApplyHazardEffect(other);
        }
    }
}
