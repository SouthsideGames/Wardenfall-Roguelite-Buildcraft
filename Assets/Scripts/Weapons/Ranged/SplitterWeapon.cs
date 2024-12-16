using UnityEngine;

public class SplitterWeapon : RangedWeapon
{
    protected override void Shoot()
    {
        OnBulletFired?.Invoke();
        anim.Play("Attack");

        int damage = GetDamage(out bool isCriticalHit);

        // Get bullet from pool and shoot it forward
        SplitterBullet bullet = bulletPool.Get() as SplitterBullet;
        bullet.Shoot(damage, transform.up, isCriticalHit);

        PlaySFX();
    }
}
