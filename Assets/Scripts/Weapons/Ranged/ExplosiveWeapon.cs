using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosiveWeapon : RangedWeapon
{
    [Header("EXPLOSIVE WEAPON SPECIFICS:")]
    [SerializeField] private float explosionRadius;

    protected override void Shoot()
    {
        Bullet explosiveBullet = bulletPool.Get();
        explosiveBullet.Shoot(damage, transform.up, _isCriticalHit: false);
        explosiveBullet.SetExplosionRadius(explosionRadius);
    }
}
