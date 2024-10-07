using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponPosition : MonoBehaviour
{
    [Header("ELEMENTS:")]
    public Weapon Weapon {get; private set;}    

    public void AssignWeapon(Weapon _weapon, int _weaponLevel)
    {
        Weapon = Instantiate(_weapon, transform);

        Weapon.transform.localPosition = Vector3.zero;
        Weapon.transform.localRotation = Quaternion.identity;

        Weapon.UpgradeTo(_weaponLevel);

    }
}
