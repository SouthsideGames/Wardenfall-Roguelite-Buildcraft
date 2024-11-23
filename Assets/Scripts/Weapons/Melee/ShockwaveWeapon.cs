using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShockwaveWeapon : MeleeWeapon
{
   [Header("SHOCKWAVE SETTINGS")]
    [SerializeField] private GameObject shockwavePrefab;   // Prefab for the shockwave
    [SerializeField] private Transform spawnPoint;         // Location to spawn the shockwave
    [SerializeField] private float shockwaveDamage = 10;   // Damage dealt by the shockwave
    [SerializeField] private float shockwaveGrowthRate = 2.0f; // How quickly the shockwave grows
    [SerializeField] private float shockwaveMaxRadius = 5.0f;  // Maximum size the shockwave can reach

    public void SpawnShockwave()
    {
        GameObject shockwave = Instantiate(shockwavePrefab, spawnPoint.position, Quaternion.identity);
        
        Shockwave shockwaveScript = shockwave.GetComponent<Shockwave>();
        shockwaveScript.enemyMask = enemyMask;
        shockwaveScript.damage = (int)shockwaveDamage;
        shockwaveScript.growthRate = shockwaveGrowthRate;
        shockwaveScript.maxRadius = shockwaveMaxRadius;
    }
}
