using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterWeapon : MonoBehaviour
{
    [Header("ELEMENTS:")]
    [SerializeField] private WeaponPosition[] weaponPositions;

    public bool TryAddWeapon(WeaponDataSO _selectedWeapon, int _weaponLevel)
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
}
