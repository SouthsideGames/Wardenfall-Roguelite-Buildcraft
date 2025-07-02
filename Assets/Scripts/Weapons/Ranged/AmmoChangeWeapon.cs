using UnityEngine;

public class AmmoChangeWeapon : RangedWeapon
{
    protected override void Shoot()
    {
        OnBulletFired?.Invoke();
        anim.Play("Attack");

        int baseDamage = GetDamage(out bool isCriticalHit);

        AdaptiveBullet bullet = bulletPool.Get() as AdaptiveBullet;
        bullet.Shoot(baseDamage, transform.up, isCriticalHit);

        PlaySFX();
    }
}
