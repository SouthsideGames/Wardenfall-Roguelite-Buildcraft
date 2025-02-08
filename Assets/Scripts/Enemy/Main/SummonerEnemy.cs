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

    private float summonTimer = 0f;
    private List<GameObject> activeSummonedEnemies = new List<GameObject>();

    protected override void Update()
    {
        base.Update();  
        
        if (!hasSpawned || !_spriteRenderer.enabled)
            return;

        HandleSummoning();

        summonTimer -= Time.deltaTime;
    }

    private void HandleSummoning()
    {
        activeSummonedEnemies.RemoveAll(enemy => enemy == null);

        if (summonTimer <= 0f && activeSummonedEnemies.Count < maxSummonedEnemies)
        {
            SummonEnemy();
            summonTimer = summonCooldown;
        }
    }

    private void SummonEnemy()
    {
        if (summonableEnemies.Length == 0 || summonPoints.Length == 0)
            return;

        GameObject enemyToSummon = summonableEnemies[Random.Range(0, summonableEnemies.Length)];

        Transform summonPoint = summonPoints[Random.Range(0, summonPoints.Length)];

        GameObject summonedEnemy = Instantiate(enemyToSummon, summonPoint.position, Quaternion.identity);

        activeSummonedEnemies.Add(summonedEnemy);

        if (summonEffect != null)
        {
            ParticleSystem effect = Instantiate(summonEffect, summonPoint.position, Quaternion.identity);
            effect.Play();
        }

    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        foreach (var point in summonPoints)
        {
            Gizmos.DrawWireSphere(point.position, 0.5f);
        }
    }
}
