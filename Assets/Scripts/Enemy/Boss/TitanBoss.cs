using UnityEngine;
using System.Collections;

public class TitanBoss : Boss
{
    [Header("Stage 1")]
    [SerializeField] private GameObject lightningBoltPrefab;
    [SerializeField] private float detectionRadius = 3f;
    //[SerializeField] private float lightningDelay = 0.5f;
    [SerializeField] private float multiAttackDelay = 0.3f;
    [SerializeField] private int lightningBurstCount = 3;
    [SerializeField] private float stageOneRotateSpeed = 5f;
    private int currentBurstCount;

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

        if (Vector2.Distance(transform.position, PlayerTransform.position) <= detectionRadius)
            ExecuteStage();
    }

    protected override void ExecuteStageOne()
    {
        if (isAttacking) return;
        isAttacking = true;
        currentBurstCount = 0;

        // Rotate around player while firing lightning
        StartCoroutine(LightningBurstRoutine());
    }

    private IEnumerator LightningBurstRoutine()
    {
        Vector2 centerPoint = PlayerTransform.position;
        float angle = 0f;

        while (currentBurstCount < lightningBurstCount)
        {
            // Calculate position in circle around player
            angle += stageOneRotateSpeed;
            Vector2 offset = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * 5f;
            transform.position = centerPoint + offset;

            SpawnLightningBolt();
            currentBurstCount++;

            yield return new WaitForSeconds(multiAttackDelay);
        }

        ResetAttack();
    }

    private void SpawnLightningBolt()
    {
        // Spawn multiple bolts in a pattern
        for (int i = 0; i < 3; i++)
        {
            Vector2 randomOffset = Random.insideUnitCircle * 2f;
            Vector2 spawnPos = (Vector2)PlayerTransform.position + randomOffset;
            Instantiate(lightningBoltPrefab, spawnPos, Quaternion.identity);
        }
    }

    protected override void ExecuteStageTwo()
    {
        if (isAttacking) return;
        isAttacking = true;

        // More aggressive pattern for stage two
        StartCoroutine(StageTwoAttackRoutine());
    }

    private IEnumerator StageTwoAttackRoutine()
    {
        // First dash to center
        enemyMovement.SetTargetPosition(centerPosition);
        yield return new WaitForSeconds(1f);

        // Spawn waves in a cross pattern
        Vector2[] directions = { Vector2.up, Vector2.right, Vector2.down, Vector2.left };
        foreach (Vector2 dir in directions)
        {
            Vector2 spawnPos = centerPosition + dir * 5f;
            Instantiate(tidalWavePrefab, spawnPos, Quaternion.LookRotation(Vector3.forward, dir));
            yield return new WaitForSeconds(0.5f);
        }

        yield return new WaitForSeconds(tidalWaveDuration);
        ResetAttack();
    }

    private void ResetAttack() => isAttacking = false;

}