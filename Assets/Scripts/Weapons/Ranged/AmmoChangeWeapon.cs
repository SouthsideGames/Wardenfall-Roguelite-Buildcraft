using UnityEngine;

public class AmmoChangeWeapon : RangedWeapon
{
    protected override void Shoot()
    {
        OnBulletFired?.Invoke();
        anim?.Play("Attack");

        int baseDamage = GetDamage(out bool isCriticalHit);

        if (bulletPool.Get() is AdaptiveBullet bullet)
        {
            bullet.Shoot(baseDamage, transform.up, isCriticalHit);
        }
        else
        {
            Debug.LogError($"{name}: bulletPool returned non-AdaptiveBullet!");
        }

        PlaySFX();
    }

}
