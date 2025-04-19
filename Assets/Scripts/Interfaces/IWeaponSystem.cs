
public interface IWeaponSystem
{
    void Attack();
    void Upgrade(float damageMultiplier);
    void SetWeapon(WeaponDataSO weaponData);
    float GetDamage();
    float GetAttackSpeed();
}
