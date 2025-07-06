using System.Collections.Generic;
using UnityEngine;

public class EnemyStatusEffectUI : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Where the status effect icons will be anchored on the enemy.")]
    [SerializeField] private Transform iconAnchor;

    [Tooltip("Prefab for individual status effect icons.")]
    [SerializeField] private StatusEffectIcon iconPrefab;

    private Dictionary<StatusEffectType, StatusEffectIcon> activeIcons = new();

    public void AddOrUpdateEffect(StatusEffect effect)
    {
        if (activeIcons.TryGetValue(effect.EffectType, out var icon))
            icon.UpdateEffect(effect);
        else
        {
            var newIcon = Instantiate(iconPrefab, iconAnchor.position, Quaternion.identity, iconAnchor);
            newIcon.Initialize(effect);
            activeIcons[effect.EffectType] = newIcon;
        }
    }

    public void RemoveEffect(StatusEffectType type)
    {
        if (activeIcons.TryGetValue(type, out var icon))
        {
            icon.PlayDispelAnimation();
            Destroy(icon.gameObject, 0.25f);
            activeIcons.Remove(type);
        }
    }

    public void ClearAll()
    {
        foreach (var icon in activeIcons.Values)
            Destroy(icon.gameObject);
            
        activeIcons.Clear();
    }


    public void ShowFloatingStatus(StatusEffectType type)
    {
        if (iconPrefab == null) return;

        Vector3 spawnPos = iconAnchor != null ? iconAnchor.position : transform.position;
        var icon = Instantiate(iconPrefab, spawnPos, Quaternion.identity);

        icon.SetupTemporary(type);
    }
}