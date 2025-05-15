using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
    Username,
    Menu,
    WeaponSelect,
    Game,
    GameOver,
    StageCompleted,
    WaveTransition,
    CardDraft,
    TraitSelection,
    Shop
}

public enum ItemRewardType
{
    Cash
}


public enum MeleeWeaponState 
{
    Empty,
    Idle,
    Attack
}

public enum UnlockCategory
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



public enum CharacterRarityType
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
    Weaken,
    Slow,
    Confuse,
    Paralyze,
    Fear,
    ArmorBreak,
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
