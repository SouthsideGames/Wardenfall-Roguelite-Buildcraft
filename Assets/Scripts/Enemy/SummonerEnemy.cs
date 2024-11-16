using System.Collections.Generic;
using UnityEngine;

public class SummonerEnemy : Enemy
{
    [Header("SUMMONER SPECIFICS:")]
    [SerializeField] private GameObject[] summonableEnemies; // Array of enemy prefabs that can be summoned
    [SerializeField] private Transform[] summonPoints; // Points around the summoner where enemies will be spawned
    [SerializeField] private float summonCooldown = 5f; // Time between summoning enemies
    [SerializeField] private int maxSummonedEnemies = 4; // Maximum number of summoned enemies at once
    [SerializeField] private ParticleSystem summonEffect; // Visual effect for summoning

    private float summonTimer = 0f; // Timer to track cooldown between summons
    private List<GameObject> activeSummonedEnemies = new List<GameObject>(); // List of currently active summoned enemies

    protected override void Update()
    {
        base.Update();  
        
        if (!hasSpawned || !_spriteRenderer.enabled)
            return;

        // Handle summoning logic
        HandleSummoning();

        // Update the summon timer
        summonTimer -= Time.deltaTime;
    }

    private void HandleSummoning()
    {
        // Remove null entries from the list (destroyed enemies)
        activeSummonedEnemies.RemoveAll(enemy => enemy == null);

        // Check if the summoner can summon more enemies
        if (summonTimer <= 0f && activeSummonedEnemies.Count < maxSummonedEnemies)
        {
            SummonEnemy();
            summonTimer = summonCooldown; // Reset the summon cooldown
        }
    }

    private void SummonEnemy()
    {
        if (summonableEnemies.Length == 0 || summonPoints.Length == 0)
            return;

        // Randomly select an enemy type to summon
        GameObject enemyToSummon = summonableEnemies[Random.Range(0, summonableEnemies.Length)];

        // Randomly select a summon point around the summoner
        Transform summonPoint = summonPoints[Random.Range(0, summonPoints.Length)];

        // Instantiate the enemy at the selected summon point
        GameObject summonedEnemy = Instantiate(enemyToSummon, summonPoint.position, Quaternion.identity);

        // Add the summoned enemy to the active list
        activeSummonedEnemies.Add(summonedEnemy);

        // Play summon effect if available
        if (summonEffect != null)
        {
            ParticleSystem effect = Instantiate(summonEffect, summonPoint.position, Quaternion.identity);
            effect.Play();
        }

    }

    private void OnDrawGizmosSelected()
    {
        // Draw summon points and detection radius in the editor for visualization
        Gizmos.color = Color.green;
        foreach (var point in summonPoints)
        {
            Gizmos.DrawWireSphere(point.position, 0.5f);
        }
    }
}
