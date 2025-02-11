using UnityEngine;

public class TitanBoss : Boss
{
    [Header("Stage 1")]
    [SerializeField] private GameObject lightningBoltPrefab;
    [SerializeField] private float detectionRadius = 3f;
    [SerializeField] private float lightningDelay = 0.5f;

    [Header("Stage 2")]
    [SerializeField] private GameObject tidalWavePrefab;
    private Vector2 centerPosition;
    [SerializeField] private float tidalWaveDuration = 3f;

    private EnemyMovement enemyMovement;
    private bool isAttacking;

    protected override void InitializeBoss()
    {
        base.InitializeBoss();
        
        enemyMovement = GetComponent<EnemyMovement>();

        Camera mainCamera = Camera.main;
        centerPosition = mainCamera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0f)); 
    }

    protected override void Update()
    {
        base.Update();

        if (!hasSpawned || isAttacking) return;

        enemyMovement.FollowCurrentTarget();

        if (Vector2.Distance(transform.position, playerTransform.position) <= detectionRadius)
            ExecuteStage();
    }

    protected override void ExecuteStageOne()
    {
        if (isAttacking) return;
        isAttacking = true;

        Invoke(nameof(SpawnLightningBolt), lightningDelay);
        Invoke(nameof(ResetAttack), 1f);
    }

    private void SpawnLightningBolt() => Instantiate(lightningBoltPrefab, playerTransform.position, Quaternion.identity);

    protected override void ExecuteStageTwo()
    {
        if (isAttacking) return;
        isAttacking = true;

        enemyMovement.SetTargetPosition(centerPosition);
        Invoke(nameof(SpawnTidalWave), 2f);
    }

    private void SpawnTidalWave()
    {
        Instantiate(tidalWavePrefab, centerPosition, Quaternion.identity);
        Invoke(nameof(ResetAttack), tidalWaveDuration);
    }

    private void ResetAttack() => isAttacking = false;

}
