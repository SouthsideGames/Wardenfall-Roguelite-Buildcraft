using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShockwaveWeapon : MeleeWeapon
{
   [Header("SHOCKWAVE SETTINGS")]
    [SerializeField, Tooltip("Prefab for the shockwave")] private GameObject shockwavePrefab;  
    [SerializeField, Tooltip("Location to spawn the shockwave")] private Transform spawnPoint;   
    [SerializeField, Tooltip("Damage dealt by the shockwave")] private float shockwaveDamage = 10;   
    [SerializeField, Tooltip("How quickly the shockwave grows")] private float shockwaveGrowthRate = 2.0f; 
    [SerializeField, Tooltip("Maximum size the shockwave can reach")] private float shockwaveMaxRadius = 5.0f;

   public void SpawnShockwave()
    {
        GameObject shockwave = Instantiate(shockwavePrefab, spawnPoint.position, Quaternion.identity);
        
        Shockwave shockwaveScript = shockwave.GetComponent<Shockwave>();
        if (shockwaveScript != null)
            shockwaveScript.Initialize(enemyMask, (int)shockwaveDamage, shockwaveGrowthRate, shockwaveMaxRadius);
    }
}
