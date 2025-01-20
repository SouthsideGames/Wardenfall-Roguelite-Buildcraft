using UnityEngine;

public static class CardEffectFactory
{
    public static ICardEffect GetEffect(CardEffectType effectType,  CardSO cardSO)
    {
        ICardEffect effect = null;

        switch (effectType)
        {
            case CardEffectType.Utility_EternalPause:
                return new EternalPauseEffect();
            case CardEffectType.Damage_FireballBarrage:
                GameObject fireballPrefab = Resources.Load<GameObject>("Prefabs/Fireball");
                return new FireballBarrageEffect(fireballPrefab, cardSO);
            case CardEffectType.Damage_EnergyBlast:
                return new EnergyBlastEffect(Resources.Load<GameObject>("Prefabs/Energy Orb"), cardSO);
            case CardEffectType.Damage_ThunderStrike:
                return new ThunderStrikeEffect(Resources.Load<GameObject>("Prefabs/Thunderbolt"), cardSO);
            case CardEffectType.Damage_ArcLightning:
                return new ArcLightningEffect(Resources.Load<GameObject>("Prefabs/LightningBolt"), cardSO);
            case CardEffectType.Damage_BladeStorm:
                return new BladeStormEffect(Resources.Load<GameObject>("Prefabs/Blade"), cardSO);
           case CardEffectType.Damage_PoisonCloud:
                return new PoisonCloudEffect(Resources.Load<GameObject>("Prefabs/Effects/Poison Cloud"), cardSO);
            case CardEffectType.Damage_PlasmaBeam:
                return new PlasmaBeamEffect(Resources.Load<GameObject>("Prefabs/PlasmaBeam"), cardSO);
            case CardEffectType.Damage_DeathRay:
                return new DeathRayEffect(Resources.Load<GameObject>("Prefabs/DeathRay"), cardSO);
            case CardEffectType.Utility_TemporalReset:
                return new TemporalResetEffect();
            case CardEffectType.Support_SecondLife:
                return new SecondLifeEffect(Resources.Load<GameObject>("Prefabs/Effects/Explosion"), cardSO);
        }

        if(effect != null && cardSO.Synergies.Count > 0)
        {
            ApplySynergies(effect, cardSO);
        }

        return effect;

    }

    private static void ApplySynergies(ICardEffect effect, CardSO cardSO)
    {
        foreach (var synergy in cardSO.Synergies)
        {
            if (CardManager.Instance.IsEffectActive(synergy.EffectType))
            {
                effect.ApplySynergy(synergy.SynergyBonus);
                Debug.Log($"Synergy applied! {synergy.EffectType} enhanced with bonus: {synergy.SynergyBonus}");
            }
        }
    }
}
