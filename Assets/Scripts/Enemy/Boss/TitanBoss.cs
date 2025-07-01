using System.Collections;
using UnityEngine;

public class TitanBoss : Boss
{
    [Header("Stage 1 Settings")]
    [SerializeField] private GameObject lightningBoltPrefab;
    [SerializeField] private float lightningDetectionRadius = 4f;
    [SerializeField] private float lightningDelay = 0.3f;
    [SerializeField] private int lightningBursts = 3;

    [Header("Stage 2 Settings")]
    [SerializeField] private GameObject tidalWavePrefab;
    [SerializeField] private float tidalWaveDuration = 3f;

    private EnemyMovement enemyMovement;
    private bool isAttacking;
    private Vector2 centerPosition;

    protected override void InitializeBoss()
    {
        base.InitializeBoss();
        enemyMovement = GetComponent<EnemyMovement>();
        centerPosition = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0f));
    }

    protected override void Update()
    {
        base.Update();

        if (!hasSpawned || isAttacking) return;

        if (GetHealthPercent() > 0.5f)
        {
            enemyMovement.FollowCurrentTarget();
            if (Vector2.Distance(transform.position, PlayerTransform.position) <= lightningDetectionRadius)
                ExecuteStageOne();
        }
        else
        {
            ExecuteStageTwo();
        }
    }

    private float GetHealthPercent()
    {
        return (float)health / maxHealth;
    }

    protected override void ExecuteStageOne()
    {
        if (isAttacking) return;
        isAttacking = true;
        StartCoroutine(LightningBurstRoutine());
    }

    private IEnumerator LightningBurstRoutine()
    {
        LeanTween.color(gameObject, Color.cyan, 0.2f).setLoopPingPong(2);
        LeanTween.scale(gameObject, transform.localScale * 1.2f, 0.2f).setEasePunch();

        int bursts = 0;
        while (bursts < lightningBursts)
        {
            SpawnLightningBolt();
            bursts++;
            yield return new WaitForSeconds(lightningDelay);
        }

        LeanTween.scale(gameObject, transform.localScale * 1.05f, 0.1f).setEasePunch();

        yield return new WaitForSeconds(1f);
        isAttacking = false;
    }

    private void SpawnLightningBolt()
    {
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
        StartCoroutine(TidalWavePhase());
    }

    private IEnumerator TidalWavePhase()
    {
        enemyMovement.canMove = false;

        LeanTween.color(gameObject, Color.blue, 0.2f).setLoopPingPong(3);
        LeanTween.scale(gameObject, transform.localScale * 1.3f, 0.2f).setEasePunch();

        // Move to center position
        enemyMovement.SetTargetPosition(centerPosition);
        yield return new WaitForSeconds(1f);

        Vector2[] directions = { Vector2.up, Vector2.right, Vector2.down, Vector2.left };
        foreach (Vector2 dir in directions)
        {
            Vector2 spawnPos = centerPosition + dir * 5f;
            Instantiate(tidalWavePrefab, spawnPos, Quaternion.LookRotation(Vector3.forward, dir));
            LeanTween.scale(gameObject, transform.localScale * 1.1f, 0.1f).setEasePunch();
            yield return new WaitForSeconds(0.5f);
        }

        yield return new WaitForSeconds(tidalWaveDuration);
        enemyMovement.canMove = true;
        isAttacking = false;
    }
}
