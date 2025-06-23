using System.Collections.Generic;
using UnityEngine;

public class SummonerEnemy : Enemy
{
    [Header("SUMMONER SPECIFICS:")]
    [Tooltip("Array of enemy prefabs that can be summoned")]
    [SerializeField] private GameObject[] summonableEnemies;

    [Tooltip("Points around the summoner where enemies will be spawned")]
    [SerializeField] private Transform[] summonPoints;

    [Tooltip("Time between summoning enemies")]
    [SerializeField] private float summonCooldown = 5f;

    [Tooltip("Maximum number of summoned enemies at once")]
    [SerializeField] private int maxSummonedEnemies = 4;

    [Tooltip("Visual effect for summoning")]
    [SerializeField] private ParticleSystem summonEffect;

    private EnemyAnimator enemyAnimator;
    private float summonTimer = 0f;
    private List<GameObject> activeSummonedEnemies = new List<GameObject>();
    
    protected override void Start()
    {
        base.Start();
        enemyAnimator = GetComponent<EnemyAnimator>();
        enemyAnimator?.PlayIdlePulseAnimation();
    }

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

        enemyAnimator?.PlaySummonAnimation(); // Play visual cue

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
