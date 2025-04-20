using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IWeaponSystem
{
    void Attack();
    void Upgrade(float damageMultiplier);
    void SetWeapon(WeaponDataSO weaponData);
    float GetDamage();
    float GetAttackSpeed();
    bool CanAttack();
    void OnWeaponEquipped();
    void OnWeaponUnequipped();
    bool IsWeaponActive();
    void ResetWeaponStats();
    float GetUpgradeCost();
    string GetWeaponDescription();
    WeaponDataSO GetWeaponData();
}
