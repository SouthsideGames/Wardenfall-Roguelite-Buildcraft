using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
    Menu,
    WeaponSelect,
    Game,
    GameOver,
    StageCompleted,
    WaveTransition,
    Shop
}

public  enum MeleeWeaponState 
{
    Idle,
    Attack
}

public enum Stat
{
    Attack,
    AttackSpeed,
    CriticalChance,
    CriticalPercent,
    MoveSpeed,
    MaxHealth,
    Range,
    HealthRecoverySpeed,
    HealthRecoveryValue,
    Armor,
    Luck,
    Dodge,
    LifeSteal,
    CriticalResistancePercent,
    PickupRange
    
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
