using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using System;

public class FlamethrowerWeapon : RangedWeapon
{
    [Header("FIRE WALL SETTINGS:")]
    [SerializeField] private GameObject fireWallPrefab;
    [SerializeField] private int fireWallDamage = 10;
    [SerializeField] private float fireWallDuration = 5f;

    protected override void Shoot()
    {
        OnBulletFired?.Invoke();
        anim.Play("Attack");

        int damage = GetDamage(out bool isCriticalHit);

        FlameBullet _bullet = (FlameBullet)bulletPool.Get();
        _bullet.SetupFireWall(fireWallPrefab, fireWallDamage, fireWallDuration);
        _bullet.Shoot(damage, transform.up, isCriticalHit);

        PlaySFX();
    }

}
