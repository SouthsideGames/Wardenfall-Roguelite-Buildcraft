using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotgunWeapon : RangedWeapon
{
    [Header("Shotgun Settings")]
    [SerializeField] private int pelletCount = 5;
    [SerializeField] private float spreadAngle = 30f;

    protected override void Shoot()
    {
        OnBulletFired?.Invoke();

        int baseDamage = GetDamage(out bool isCriticalHit);

        for (int i = 0; i < pelletCount; i++)
        {
            float angle = Random.Range(-spreadAngle / 2, spreadAngle / 2);
            Vector2 direction = Quaternion.Euler(0, 0, angle) * transform.up;

            BulletBase pellet = bulletPool.Get();
            pellet.Shoot(baseDamage, direction, isCriticalHit);
        }

        PlaySFX();
    }
}
