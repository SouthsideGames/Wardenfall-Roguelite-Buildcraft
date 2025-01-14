using UnityEngine;

public static class CardEffectFactory
{
    public static ICardEffect GetEffect(CardEffectType effectType)
    {
        switch (effectType)
        {
            case CardEffectType.Utility_EternalPause:
                return new EternalPauseEffect();
            case CardEffectType.Damage_FireballBarrage:
                GameObject fireballPrefab = Resources.Load<GameObject>("Prefabs/Fireball");
                return new FireballBarrageEffect(fireballPrefab, fireballCount: 5, spawnRadius: 10f, baseDamage: 50, isCriticalHit: false);
            default:
                Debug.LogWarning($"No effect defined for {effectType}.");
                return null;
        }
    }
}
