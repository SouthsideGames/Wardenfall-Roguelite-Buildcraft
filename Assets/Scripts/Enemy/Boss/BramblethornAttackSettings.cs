using UnityEngine;

[System.Serializable]
public class BramblethornAttackSettings
{
     [Header("General Settings")]
    public float attackPauseTime = 2f;
    public float regrowthThreshold = 0.5f; // 50% HP

    [Header("Root Slam")]
    public float rootSlamRadius = 3.5f;
    public int slamDamage = 30;
    public float rootSlamCooldown = 8f;

    [Header("Thorn Barrage")]
    public int thornProjectileCount = 6;
    public float thornProjectileSpeed = 10f;
    public float thornBarrageCooldown = 6f;
    public GameObject thornProjectilePrefab;
    public Transform thornSpawnPoint;

    [Header("Charge Attack")]
    public float chargeCooldown = 12f;
}
