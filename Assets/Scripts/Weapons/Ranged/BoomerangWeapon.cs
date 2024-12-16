using UnityEngine;

public class BoomerangWeapon : RangedWeapon
{
    [Header("Boomerang Settings")]
    [SerializeField, Tooltip("Max travel distance")] private float maxDistance = 7f;
    [SerializeField, Tooltip("Return speed")] private float returnSpeedMultiplier = 1.5f; 

    protected override void Shoot()
    {
        OnBulletFired?.Invoke();
        anim.Play("Attack");

        int damage = GetDamage(out bool isCriticalHit);

        BoomerangBullet bullet = bulletPool.Get() as BoomerangBullet;
        bullet.Shoot(damage, transform.up, isCriticalHit, maxDistance, returnSpeedMultiplier);

        PlaySFX();
    }
}
