using UnityEngine;

/// <summary>
/// Base class for all environmental hazards that can damage the player
/// Acts as an abstract template for specific hazard types
/// </summary>
public abstract class EnvironmentalHazard : MonoBehaviour
{
    [Header("Hazard Settings")]
    [SerializeField] protected int damageAmount = 10;      // Amount of damage dealt to player
    [SerializeField] protected float damageInterval = 1f;  // Time between damage ticks
    [SerializeField] protected float lifetime = -1f;       // How long hazard exists (-1 for permanent)

    protected float nextDamageTime;                        // Tracks when next damage can be applied

    /// <summary>
    /// Initialize hazard and handle lifetime if temporary
    /// </summary>
    protected virtual void Start()
    {
        if (lifetime > 0)
        {
            Destroy(gameObject, lifetime);
        }
    }

    /// <summary>
    /// Checks for player collision and applies damage based on interval
    /// </summary>
    protected virtual void OnTriggerStay2D(Collider2D other)
    {
        // Only damage player and respect damage interval timing
        if (other.CompareTag("Player") && Time.time >= nextDamageTime)
        {
            ApplyHazardEffect(other);
            nextDamageTime = Time.time + damageInterval;
        }
    }

    /// <summary>
    /// Applies the hazard's damage effect to the colliding object
    /// Can be overridden by specific hazards to add additional effects
    /// </summary>
    protected virtual void ApplyHazardEffect(Collider2D other)
    {
        CharacterManager.Instance.health.TakeDamage(damageAmount);
    }
}