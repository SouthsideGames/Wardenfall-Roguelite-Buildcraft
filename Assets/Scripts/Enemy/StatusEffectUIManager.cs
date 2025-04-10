using System.Collections.Generic;
using UnityEngine;

public class StatusEffectUIManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform iconAnchor;
    [SerializeField] private StatusEffectIcon iconPrefab;

    private Dictionary<StatusEffectType, StatusEffectIcon> activeIcons = new();

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
        {
            Destroy(icon.gameObject);
        }
        activeIcons.Clear();
    }
}