using UnityEngine;
using System;

public class SplitterWeapon : RangedWeapon
{
    public static Action OnAmmoAFinished;

    private bool canFire = true;

    private void OnEnable() => OnAmmoAFinished += ResetFiring;
    private void OnDisable() => OnAmmoAFinished -= ResetFiring;
    protected override void Shoot()
    {
        if (!canFire)
            return;

        OnBulletFired?.Invoke();
        anim.Play("Attack");

        int damage = GetDamage(out bool isCriticalHit);

        SplitterBullet bullet = bulletPool.Get() as SplitterBullet;
        bullet.Shoot(damage, transform.up, isCriticalHit);

        canFire = false; 
        PlaySFX();
    }

    private void ResetFiring() => canFire = true; 
}
