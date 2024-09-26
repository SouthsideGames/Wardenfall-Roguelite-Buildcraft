using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
    MENU,
    WEAPONSELECT,
    GAME,
    GAMEOVER,
    STAGECOMPLETED,
    WAVETRANSITION,
    SHOP
}

public  enum MeleeWeaponState 
{
    IDLE,
    ATTACK
}

public enum PlayerStat
{
    Attack,
    AttackSpeed,
    CriticalChance,
    CriticalPercent,
    MoveSpeed,
    MaxHealth,
    Range,
    HealthRecoverySpeed,
    Armor,
    Luck,
    Dodge,
    LifeSteal,
    CriticalResistance,
    PickupRange,
    ElementalDamage
    
}

public static class Enums
{
    public static string FormatStatName(PlayerStat _playerStat)
    {
        string formated = "";
        string unformatedString = _playerStat.ToString();

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
