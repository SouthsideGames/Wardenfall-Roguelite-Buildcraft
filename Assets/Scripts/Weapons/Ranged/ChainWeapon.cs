using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ChainWeapon : RangedWeapon
{ 
    [Header("CHAIN SETTINGS")]
    [SerializeField] private float chainRadius = 5f;
    [SerializeField] private int maxChains = 4;
    [SerializeField] private float damageFalloff = 0.75f;

    protected override void Shoot()
    {
        OnBulletFired?.Invoke();
        anim.Play("Attack");

        int damage = GetDamage(out bool isCriticalHit);

        ChainBullet bullet = bulletPool.Get() as ChainBullet;
        bullet.Initialize(damage, transform.up, isCriticalHit, chainRadius, maxChains, damageFalloff, enemyMask);

        PlaySFX();
    }

}
