using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

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
            //Cant Fuse with a null weapon
            if(characterWeapon == null)
               continue;
            
            //Cant fuse a weapon to itself
            if(characterWeapon == _weapon)
               continue;

            //Can not fuse the same weapons
            if(characterWeapon.WeaponData.Name != _weapon.WeaponData.Name)
                continue;

            //both weapons need to be the same level
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

        onFuse?.Invoke(weapon);
    }

}
