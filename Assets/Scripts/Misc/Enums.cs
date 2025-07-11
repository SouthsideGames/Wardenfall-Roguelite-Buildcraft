using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
    Intro,
    Username,
    Menu,
    WeaponSelect,
    Game,
    GameOver,
    Progression,
    StageCompleted,
    WaveTransition,
    CardDraft,
    TraitSelection,
    Shop
}

public enum ItemRewardType
{
    Cash,
    CardPoints,
    UnlockTickets

}


public enum MeleeWeaponState 
{
    Empty,
    Idle,
    Attack
}

public enum ProgressionUnlockCategory
{
    StatBooster,
    ShopEconomy,
    Card,
    Perk,
    Special
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
    Attacking,
    Transitioning
}


public enum RewardType
{
    Currency_01 = 0,
    Currency_02 = 1,
    Special_01 = 200
}

public enum DeviceType
{
    Mobile,
    Tablet,
    Desktop
}

public enum CharacterRarityType
{
    Common,
    Uncommon,
    Rare,
    Epic,
    Legendary
}

public enum EnemyType
{
    Melee,
    Explode,
    Berserker,
    Shield,
    Charge,
    Shooter,
    Healer,
    Summoner,
    Splitter,
    Teleport,
    Elite
}

public enum StatusEffectType
{
    Burn,
    Drain,
    Stun,
    Freeze,
    Poison,
    Weaken,
    Slow,
    Confuse,
    Paralyze,
    Fear,
    DeathMark
}

public enum AbilityType
{
    FlameJet,
    LuckyRoll,
    TargetLockDash,
    SlipstreamSurge,
    BeastLunge,
    QuantumBlink,
    UppercutDash,
    Shadowstep,
    GhostGlide,
    SoulFlicker
}

public enum StackBehavior
{
    Refresh,
    Extend,
    AddValue
}

public enum DraftType 
{ 
    Major, 
    Mini 
}

public enum CrowdReactionType
{
    Cheer,
    Gasp,
    Roar,
    Boo,
    Laugh,
    Whistle,
    Ouch,
}

public enum ChallengeMode
{
    None,
    Hardcore,
    TraitChaos,
    RogueRoulette,
    SingleSlot
}

public enum RotationMode
{
    Instant,
    SmoothLerp,
    LeanTween
}


public static class Enums
{
    public static string FormatStatName(Stat _stat)
    {
        string formated = "";
        string unformatedString = _stat.ToString();

        if (unformatedString.Length <= 0)
            return "Unvalid Stat Name";

        formated += unformatedString[0];

        for (int i = 1; i < unformatedString.Length; i++)
        {
            if (char.IsUpper(unformatedString[i]))
                formated += " ";

            formated += unformatedString[i];
        }


        return formated;
    }
}
