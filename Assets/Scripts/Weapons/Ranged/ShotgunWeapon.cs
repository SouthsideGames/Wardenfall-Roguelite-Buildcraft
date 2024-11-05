using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotgunWeapon : RangedWeapon
{
    [Header("SHOTGUN WEAPON SPECIFICS:")]
    [SerializeField] private int pelletCount;
    [SerializeField] private float spreadAngle;

    protected override void Shoot()
    {
        for (int i = 0; i < pelletCount; i++)
        {
            float angleOffset = spreadAngle * (i - pelletCount / 2) / pelletCount;
            Vector2 direction = Quaternion.Euler(0, 0, angleOffset) * transform.up;

            Bullet pellet = bulletPool.Get();
            pellet.Shoot(damage, direction, _isCriticalHit: false);
        }
    }
}
