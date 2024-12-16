using UnityEngine;

public class RicochetWeapon : RangedWeapon
{
    [Header("RICHOCHET SETTINGS")]
    [SerializeField] private float randomAngleRange = 360f;

    protected override void AutoAimLogic() => ShootLogic();

    protected override void Shoot()
    {
        OnBulletFired?.Invoke();
        anim.Play("Attack");

        int damage = GetDamage(out bool isCriticalHit);

        // Generate a random firing angle
        float randomAngle = Random.Range(0f, randomAngleRange);
        Vector2 randomDirection = Quaternion.Euler(0, 0, randomAngle) * Vector2.up;

        // Rotate the weapon to face the shooting direction
        transform.up = randomDirection;

        RicochetBullet bullet = bulletPool.Get() as RicochetBullet;
        bullet.Shoot(damage, randomDirection, isCriticalHit);

        PlaySFX();
    }
}
