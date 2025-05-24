using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using SouthsideGames.DailyMissions;

public class WeaponFuserManager : MonoBehaviour
{
    public static WeaponFuserManager Instance;

    [Header("ELEMENTS:")]
    [SerializeField] private CharacterWeapon characterWeapon;
    private List<Weapon> weaponsToFuse = new List<Weapon>();    

    public static Action<Weapon> onFuse;

    private void Awake()
    {
        if(Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public bool CanFuse(Weapon _weapon)
    {
        if(_weapon.Level >= 3)
        return false;

        weaponsToFuse.Clear();
        weaponsToFuse.Add(_weapon); 

        Weapon[] weapons = characterWeapon.GetWeapons();

        foreach(Weapon characterWeapon in weapons)
        {
            if(characterWeapon == null)
               continue;

            if(characterWeapon == _weapon)
               continue;

            if(characterWeapon.WeaponData.Name != _weapon.WeaponData.Name)
                continue;

            if(characterWeapon.Level != _weapon.Level)
               continue;

            weaponsToFuse.Add(characterWeapon);
            
            return true;
        }

        return false;
    }

    public void Fuse()
    {
        if(weaponsToFuse.Count < 2)
            return;

        DestroyImmediate(weaponsToFuse[1].gameObject);

        weaponsToFuse[0].Upgrade();

        Weapon weapon = weaponsToFuse[0];
        weaponsToFuse.Clear();

        FusionDividendAction();

        MissionManager.Increment(MissionType.fusionFanatic, 1);

        onFuse?.Invoke(weapon);
    }

    private void FusionDividendAction()
    {
        int fusionCash = ProgressionManager.Instance.progressionEffectManager.FusionCashReward;
        if (fusionCash > 0)
        {
            CurrencyManager.Instance.AdjustPremiumCurrency(fusionCash);
        }
    }

}
