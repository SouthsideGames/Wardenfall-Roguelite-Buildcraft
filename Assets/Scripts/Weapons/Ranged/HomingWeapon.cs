using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomingWeapon : RangedWeapon
{
    [Header("HOMING SETTINGS")]
    [SerializeField] private float homingSpeed = 5.0f;
    [SerializeField] private float maxHomingRange = 10.0f;

    protected override void Shoot()
    {
        Enemy target = GetClosestEnemy();
        if (target != null)
        {
            Bullet missile = bulletPool.Get();
            missile.Shoot(damage, transform.up, false);
            missile.SetHomingTarget(target, homingSpeed);
        }
    }
}
