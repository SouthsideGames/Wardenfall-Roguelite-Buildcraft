using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class WeaponStatCalculator
{
    public static Dictionary<Stat, float> GetStats(WeaponDataSO _weaponDataSO, int _level)
    {
        float multiplier = 1 + (float)_level / 3;

        Dictionary<Stat, float> calculatedStats = new Dictionary<Stat, float>();

        foreach(KeyValuePair<Stat, float> kvp in _weaponDataSO.BaseStats)
        {
            
            if(_weaponDataSO.Prefab.GetType() != typeof(RangedWeapon) && kvp.Key == Stat.Range)
                calculatedStats.Add(kvp.Key, kvp.Value);
            else
                calculatedStats.Add(kvp.Key, kvp.Value * multiplier);
        }
         
        

        return calculatedStats;
    }

    public static int GetPurchasePrice(WeaponDataSO _weaponData, int _level)
    {
        float multiplier = 1 + (float)_level / 3;
        return (int)(_weaponData.PurchasePrice * multiplier);
    } 

    public static int GetRecyclePrice(WeaponDataSO _weaponData, int _level)
    {
        float multiplier = 1 + (float)_level / 3;
        return (int)(_weaponData.RecyclePrice * multiplier);
    } 
}
