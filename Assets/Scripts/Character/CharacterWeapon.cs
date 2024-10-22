using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterWeapon : MonoBehaviour
{
    [Header("ELEMENTS:")]
    [SerializeField] private WeaponPosition[] weaponPositions;

    public bool AddWeapon(WeaponDataSO _selectedWeapon, int _weaponLevel)
    {
        for (int i = 0; i < weaponPositions.Length; i++)
        {
            if(weaponPositions[i].Weapon != null)
               continue;

            weaponPositions[i].AssignWeapon(_selectedWeapon.Prefab, _weaponLevel);
            return true;    
        }


        return false;
    }

    public Weapon[] GetWeapons()
    {
        List<Weapon> weaponList = new List<Weapon>();  

        foreach (WeaponPosition weaponPosition in weaponPositions)
        {
            if(weaponPosition.Weapon == null)   
                continue;   
            
            weaponList.Add(weaponPosition.Weapon);
        }

        return weaponList.ToArray();    
    }

    public void RecycleWeapon(int _weaponIndex)
    {
        for(int i = 0; i < weaponPositions.Length; i++)
        {
            if(i != _weaponIndex)
               continue;

            int recyclePrice = weaponPositions[i].Weapon.GetWeaponRecyclePrice();
            CurrencyManager.Instance.AdjustCurrency(recyclePrice);

            weaponPositions[i].RemoveWeaponFromPosition();

            return;
        }
    }
}
