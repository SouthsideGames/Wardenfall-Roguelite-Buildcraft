using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HiveTyrantBoss : Boss
{   
    [Header("STAGE 1")]
    [SerializeField] private float dashCooldown = 3f;
    [SerializeField] private float dashDistance = 3f;
    
    [Header("STAGE 2")]
    [SerializeField] private GameObject stingletPrefab;
    [SerializeField] private int maxMinions = 3;
    [SerializeField] private float spawnInterval = 5f;

    private int activeMinions = 0;
    private EnemyMovement enemyMovement;
    private Vector3 originalScale;
    private bool isDashing;
    private float dashTimer;
    private float spawnTimer;

    protected override void InitializeBoss()
    {
        base.InitializeBoss();
        enemyMovement = GetComponent<EnemyMovement>();
        originalScale = transform.localScale;

        dashTimer = dashCooldown;
        spawnTimer = spawnInterval;
    }

    protected override void Update()
    {
        base.Update();

        if (!hasSpawned || isDashing) return;

        dashTimer -= Time.deltaTime;
        spawnTimer -= Time.deltaTime;

        if (dashTimer <= 0)
        {
            ExecuteStage(); 
            dashTimer = dashCooldown;
        }
    }

    protected override void ExecuteStageOne() =>  PerformDash();
    protected override void ExecuteStageTwo() =>  SpawnMinion();

    private void PerformDash()
    {
        if (isDashing) return;

        isDashing = true;
        enemyMovement.DisableMovement(0.6f);

        transform.localScale = new Vector3(originalScale.x * 1.5f, originalScale.y * 0.7f, originalScale.z);

        Vector2 dashDirection = (PlayerTransform.position - transform.position).normalized;
        Vector2 dashTarget = (Vector2)transform.position + dashDirection * dashDistance;

        enemyMovement.SetTargetPosition(dashTarget);
        Invoke(nameof(ResetAfterDash), 0.5f);
    }

    private void ResetAfterDash()
    {
        transform.localScale = originalScale;
        isDashing = false;
    }

    private void SpawnMinion()
    {
        if (activeMinions >= maxMinions) return;

        Instantiate(stingletPrefab, transform.position, Quaternion.identity);
        activeMinions++;
    }

}
