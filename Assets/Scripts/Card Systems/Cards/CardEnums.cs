using UnityEngine;

    public enum CardType
    {
        None,
        Damage,
        Utility,
        Support,
        Special
    }

    public enum CardRarityType
    {
        Common,
        Uncommon,
        Rare,
        Epic,
        Legendary,
        Mythic,
        Exalted
    }

    public enum MiniCardRarityType
    {
        Common,
        Uncommon,
        Rare,
        Epic,
        Legendary,
        Mythic,
        Exalted
    }

    public enum CardEffectType
    {
        Damage_FireballBarrage = 001,
        Damage_EnergyBlast = 002,
        Damage_ThunderStrike = 003,
        Damage_ArcLightning = 004,
        Damage_BladeStorm = 005,
        Damage_PoisonCloud = 006,
        Damage_PlasmaBeam = 007,
        Damage_DeathRay = 008,
        Damage_GravityCollapse = 009,
        Support_SecondLife = 100,
        Support_PrecisionFocus = 101,
        Support_AdrenalineSurge = 102,
        Support_BerserkerRage = 103,
        Support_GuardianSpirit = 104,
        Utility_EternalPause = 200,
        Utility_TemporalReset = 201,
        Utility_DoubleItemValue = 202,
        Utility_ItemBooster = 203,
        Utility_EnergyLink = 204,
        Special_MoltenTrail = 300,
        Special_MinionSwarm = 301,
        Special_NecromancersCall = 302
    }

