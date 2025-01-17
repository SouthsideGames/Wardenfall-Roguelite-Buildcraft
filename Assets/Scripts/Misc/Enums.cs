using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
    Menu,
    WeaponSelect,
    GameModeSelect,
    Game,
    GameOver,
    StageCompleted,
    SurvivalStageCompleted,
    WaveTransition,
    Shop
}

public enum ItemRewardType
{
    Cash,
    Gem,
    Card
}


public enum MeleeWeaponState 
{
    Empty,
    Idle,
    Attack
}

public enum Stat
{
    Attack,
    AttackSpeed,
    CritChance,
    CritDamage,
    MoveSpeed,
    MaxHealth,
    Range,
    RegenSpeed,
    RegenValue,
    Armor,
    Luck,
    Dodge,
    LifeSteal,
    CritResist,
    PickupRange
    
}

public enum BossState
{
    None,
    Idle,
    Moving,
    Attacking
}

public enum GameMode
{
    WaveBased,
    Survival,
    BossRush
}

public enum MissionType
{
    wavesCompleted = 0,
    enemiesPopped = 1,
    premiumCurrencyCollected = 2,
    currencyCollected = 3,
    waveBasedPlayed = 4,
    survivalPlayed = 5,
    bossRushPlayed = 6,
    gemCollected = 7
    
}

public enum RewardType
{
    Currency_01 = 0,
    Currency_02 = 1,
    Currency_03 = 2,

    Special_01 = 200,
    Special_02 = 201
}

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
    None,
    Utility_EternalPause,
    Damage_FireballBarrage,
    Damage_EnergyBlast,
    Damage_ThunderStrike,
    Damage_ArcLightning,
    Damage_BladeStorm,
    Damage_PoisonCloud,
    Damage_PlasmaBeam,
    Damage_DeathRay
}

public enum CharacterCardRarityType
{
    Common,
    Uncommon,
    Rare,
    Epic,
    Legendary
}


public enum StatusEffectType
{
    Burn,
    Drain,
    Stun,
    Freeze,
    Poison,
    Blind,
    Weaken,
    Slow,
    Confuse,
    Paralyze,
    Fear,
    ArmorBreak,
    Silence
}


public static class Enums
{
    public static string FormatStatName(Stat _stat)
    {
        string formated = "";
        string unformatedString = _stat.ToString();

        if(unformatedString.Length <= 0)
            return "Unvalid Stat Name";

        formated += unformatedString[0];

        for (int i = 1; i < unformatedString.Length; i++)
        {
            if(char.IsUpper(unformatedString[i]))
              formated += " ";

            formated += unformatedString[i];    
        }


        return formated;
    }
}
