using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles persistent status effect icons on enemy units.
/// Icons are anchored to a world-space transform and update over time.
/// </summary>
public class EnemyStatusEffectUI : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Where the status effect icons will be anchored on the enemy.")]
    [SerializeField] private Transform iconAnchor;

    [Tooltip("Prefab for individual status effect icons.")]
    [SerializeField] private StatusEffectIcon iconPrefab;

    private Dictionary<StatusEffectType, StatusEffectIcon> activeIcons = new();

    /// <summary>
    /// Adds a new status effect icon or updates the existing one.
    /// </summary>
    public void AddOrUpdateEffect(StatusEffect effect)
    {
        if (activeIcons.TryGetValue(effect.EffectType, out var icon))
        {
            icon.UpdateEffect(effect);
        }
        else
        {
            var newIcon = Instantiate(iconPrefab, iconAnchor.position, Quaternion.identity, iconAnchor);
            newIcon.Initialize(effect);
            activeIcons[effect.EffectType] = newIcon;
        }
    }

    /// <summary>
    /// Removes the icon for a specific status effect.
    /// </summary>
    public void RemoveEffect(StatusEffectType type)
    {
        if (activeIcons.TryGetValue(type, out var icon))
        {
            icon.PlayDispelAnimation();
            Destroy(icon.gameObject, 0.25f);
            activeIcons.Remove(type);
        }
    }

    /// <summary>
    /// Clears all active status effect icons.
    /// </summary>
    public void ClearAll()
    {
        foreach (var icon in activeIcons.Values)
        {
            Destroy(icon.gameObject);
        }
        activeIcons.Clear();
    }

    /// <summary>
    /// (Optional Extension)
    /// Call this if you want to show a temporary floating icon from a direct hit or area effect.
    /// </summary>
    public void ShowFloatingStatus(StatusEffectType type)
    {
        if (StatusEffectUIManager.Instance != null)
        {
            StatusEffectUIManager.Show(type, transform.position + Vector3.up * 1f);
        }
    }
}