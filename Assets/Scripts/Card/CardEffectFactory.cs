using UnityEngine;

public static class CardEffectFactory
{
    public static ICardEffect GetEffect(CardEffectType effectType,  CardSO cardSO)
    {
        switch (effectType)
        {
            case CardEffectType.Utility_EternalPause:
                return new EternalPauseEffect();
            case CardEffectType.Damage_FireballBarrage:
                GameObject fireballPrefab = Resources.Load<GameObject>("Prefabs/Fireball");
                return new FireballBarrageEffect(fireballPrefab, fireballCount: 5, spawnRadius: 10f, baseDamage: 50, isCriticalHit: false);
            case CardEffectType.Damage_EnergyBlast:
                return new EnergyBlastEffect(Resources.Load<GameObject>("Prefabs/Energy Orb"));
            case CardEffectType.Damage_ThunderStrike:
                return new ThunderStrikeEffect(Resources.Load<GameObject>("Prefabs/Thunderbolt"));
            case CardEffectType.Damage_ArcLightning:
                return new ArcLightningEffect(Resources.Load<GameObject>("Prefabs/LightningBolt"));
            case CardEffectType.Damage_BladeStorm:
                return new BladeStormEffect(Resources.Load<GameObject>("Prefabs/Blade"), 4, 5f);
           case CardEffectType.Damage_PoisonCloud:
                return new PoisonCloudEffect(
                    Resources.Load<GameObject>("Prefabs/Effects/Poison Cloud"),
                    cardSO
                );
            default:
                Debug.LogWarning($"No effect defined for {effectType}.");
                return null;
        }
    }
}
