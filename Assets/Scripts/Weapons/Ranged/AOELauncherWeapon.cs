using UnityEngine;

public class AOELauncherWeapon : RangedWeapon
{
   [Header("AOE SETTINGS")]
    [SerializeField] private float explosionRadius = 3f;

    protected override void Shoot()
    {
        OnBulletFired?.Invoke();
        anim.Play("Attack");

        int damage = GetDamage(out bool isCriticalHit);

        HomingRocket bullet = bulletPool.Get() as HomingRocket;
        bullet.Initialize(damage, transform.up, isCriticalHit, explosionRadius, enemyMask);

        PlaySFX();
    }
}
