using UnityEngine;

public class TitanBoss : Boss
{
    [Header("Stage 1")]
    [SerializeField] private GameObject lightningBoltPrefab;
    [SerializeField] private float detectionRadius = 3f;
    [SerializeField] private float lightningDelay = 0.5f;

    [Header("Stage 2")]
    [SerializeField] private GameObject tidalWavePrefab;
    [SerializeField] private Transform centerPosition;
    [SerializeField] private float tidalWaveDuration = 3f;
    [SerializeField] private float healthThreshold = 0.5f; // 50% HP to trigger stage 2
    
    private EnemyMovement enemyMovement;
    private bool isAttacking;
    private bool hasTransitioned = false;

    protected override void InitializeBoss()
    {
        base.InitializeBoss();
        enemyMovement = GetComponent<EnemyMovement>();
    }

    protected override void Update()
    {
        base.Update();

        if (!hasSpawned || isAttacking) return;

        if (!hasTransitioned && (float)health / maxHealth <= healthThreshold)
            AdvanceToNextStage();
        else
        {
            enemyMovement.FollowCurrentTarget();

            if (Vector2.Distance(transform.position, playerTransform.position) <= detectionRadius)
                ExecuteStageOne();
        }
    }

    protected override void ExecuteStageOne()
    {
        if (isAttacking) return;
        isAttacking = true;

        Debug.Log("Tidal Titan: Lightning Strike!");
        Vector2 targetPosition = playerTransform.position;
        
        Invoke(nameof(SpawnLightningBolt), lightningDelay);
        Invoke(nameof(ResetAttack), 1f);
    }

    private void SpawnLightningBolt() => Instantiate(lightningBoltPrefab, playerTransform.position, Quaternion.identity);
    

    private void AdvanceToNextStage()
    {
        if (hasTransitioned) return;
        hasTransitioned = true;
        stageToExecute = 2;

        enemyMovement.SetTargetPosition(centerPosition.position);
        Invoke(nameof(ExecuteStageTwo), 2f);
    }

    protected override void ExecuteStageTwo()
    {
        isAttacking = true;
        Debug.Log("Tidal Titan: Summoning Tidal Wave!");

        Instantiate(tidalWavePrefab, centerPosition.position, Quaternion.identity);
        Invoke(nameof(ResetAttack), tidalWaveDuration);
    }

    private void ResetAttack() => isAttacking = false;
}
