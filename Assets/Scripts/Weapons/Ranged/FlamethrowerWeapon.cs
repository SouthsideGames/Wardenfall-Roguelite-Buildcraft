using UnityEngine;

public class FlamethrowerWeapon : RangedWeapon
{
    [Header("WAVE SETTINGS:")]
    [SerializeField] private GameObject fireWavePrefab;
    [SerializeField] private float waveSpeed = 10f;
    [SerializeField] private float waveRange = 8f;
    [SerializeField] private float waveWidth = 1f;
    [SerializeField] private int burnDamage = 5;
    [SerializeField] private float burnDuration = 3f;
    [SerializeField] private float burnInterval = 1f;

    protected override void Shoot()
    {
        OnBulletFired?.Invoke();
        anim.Play("Attack");
        PlaySFX();

        // Create and configure the fire wave
        GameObject fireWave = Instantiate(fireWavePrefab, firePoint.position, transform.rotation);
        FireWave fireWaveScript = fireWave.GetComponent<FireWave>();
        fireWaveScript.Setup(waveSpeed, waveRange, burnDamage, burnDuration, burnInterval, waveWidth);
    }
}
