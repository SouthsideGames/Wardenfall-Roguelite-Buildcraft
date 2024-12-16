using UnityEngine;

public class BulletRainLauncherWeapon : RangedWeapon
{
   [Header("TAGALONG SETTINGS")]
    [SerializeField] private float explosionRadius = 3f;

    protected override void Shoot()
    {
        OnBulletFired?.Invoke();
        anim.Play("Attack");

        int damage = GetDamage(out bool isCriticalHit);

        HomingAOEBullet bullet = bulletPool.Get() as HomingAOEBullet;
        bullet.Initialize(damage, transform.up, isCriticalHit, explosionRadius, enemyMask);

        PlaySFX();
    }
}
