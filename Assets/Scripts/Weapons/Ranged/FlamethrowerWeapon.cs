using UnityEngine;

public class FlamethrowerWeapon : RangedWeapon
{
    [Header("WAVE SETTINGS:")]
    [SerializeField] private GameObject fireWavePrefab; // Prefab for the fire wave
    [SerializeField] private float waveSpeed = 10f;     // Speed of the wave
    [SerializeField] private float waveRange = 8f;      // Max travel distance
    [SerializeField] private float burnDuration = 3f;   // Burn effect duration
    [SerializeField] private float burnDamage = 5f;     // Damage per second from burn effect
    [SerializeField] private float waveWidth = 1f;      // Width of the wave

    protected override void Shoot()
    {
        // Trigger animation and sound
        OnBulletFired?.Invoke();
        anim.Play("Attack");
        PlaySFX();

        // Calculate damage
        int damage = GetDamage(out bool isCriticalHit);

        // Create the fire wave
        GameObject fireWave = Instantiate(fireWavePrefab, firePoint.position, transform.rotation);
        FireWave fireWaveScript = fireWave.GetComponent<FireWave>();

        // Configure fire wave settings
        fireWaveScript.Setup(damage, waveSpeed, waveRange, burnDamage, burnDuration, waveWidth, isCriticalHit);
    }
}
