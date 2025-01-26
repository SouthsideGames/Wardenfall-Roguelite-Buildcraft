using UnityEngine;

public static class CardEffectFactory
{
    public static ICardEffect GetEffect(CardEffectType effectType,  CardSO cardSO)
    {
        switch (effectType)
        {
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
            case CardEffectType.Damage_GravityCollapse:
                return new GravityCollapseEffect(Resources.Load<GameObject>("Prefabs/Effects/Gravity Field"), cardSO, _pullRadius: 5f);
            case CardEffectType.Utility_EternalPause:
                return new EternalPauseEffect();
            case CardEffectType.Utility_TemporalReset:
                return new TemporalResetEffect();
            case CardEffectType.Utility_DoubleItemValue:
                return new DoubleItemValueEffect(cardSO.ActiveTime);
            case CardEffectType.Utility_ItemBooster:
                return new ItemBoosterEffect(cardSO.EffectValue);
            case CardEffectType.Support_SecondLife:
                return new SecondLifeEffect(Resources.Load<GameObject>("Prefabs/Effects/Explosion"), cardSO);
            case CardEffectType.Support_PrecisionFocus:
                return new PrecisionFocusEffect(cardSO);
            case CardEffectType.Support_AdrenalineSurge:
                return new AdrenalineSurgeEffect(cardSO);
            case CardEffectType.Support_BerserkerRage:
                return new BerserkerRageEffect(cardSO);
            case CardEffectType.Support_GuardianSpirit:
                return new GuardianSpiritEffect(Resources.Load<GameObject>("Prefabs/Allies/GuardianSpirit"), CharacterManager.Instance.transform, cardSO.EffectValue);

            case CardEffectType.Special_MoltenTrail:
                return new MoltenTrailEffect(Resources.Load<GameObject>("Prefabs/Effects/MoltenTrail"), cardSO);
            case CardEffectType.Special_MinionSwarm:
                GameObject minionPrefab = Resources.Load<GameObject>("Prefabs/Allies/Minion");
                 return new MinionSwarmEffect(minionPrefab, CharacterManager.Instance.transform, _minionDamage: (int)cardSO.EffectValue, _minionCount: 5, _spawnInterval: 1f);
            case CardEffectType.Special_NecromancersCall:
                return new NecromancerCallEffect(Resources.Load<GameObject>("Prefabs/Allies/Undead Minion"),CharacterManager.Instance.transform, cardSO);
            default:
                Debug.LogWarning($"No effect defined for {effectType}.");
                return null;
        }

    }

}
