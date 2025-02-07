using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HiveTyrantBoss : Boss
{   
   [Header("STAGE 1 - Dashing")]
    [SerializeField] private float dashCooldown = 3f;
    [SerializeField] private float dashDistance = 3f;
    
    [Header("STAGE 2 - Enraged Mode")]
    [SerializeField] private float enragedDashCooldown = 2f;
    [SerializeField] private float enragedSpawnInterval = 3f;
    [SerializeField] private float enragedSpeedMultiplier = 1.3f;
    [SerializeField] private float stageTwoHealthThreshold = 0.5f; 

    [Header("Minion Spawning")]
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

        // Transition to Stage 2 if health is low
        if (currentStage == 1 && (float)health / maxHealth <= stageTwoHealthThreshold)
        {
            AdvanceToNextStage();
        }

        dashTimer -= Time.deltaTime;
        spawnTimer -= Time.deltaTime;

        if (dashTimer <= 0)
        {
            ExecuteStage();  // **Automatically calls the correct attack phase**
            dashTimer = (currentStage == 2) ? enragedDashCooldown : dashCooldown;
        }

        if (currentStage == 2 && spawnTimer <= 0 && activeMinions < maxMinions)
        {
            ExecuteStageTwo(); // Only spawn minions in Stage 2
            spawnTimer = enragedSpawnInterval;
        }
    }

    protected override void ExecuteStageOne()
    {
        PerformDash();
    }

    protected override void ExecuteStageTwo()
    {
        SpawnMinion();
    }

    private void PerformDash()
    {
        if (isDashing) return;

        isDashing = true;
        enemyMovement.DisableMovement(0.6f);
        Debug.Log("Hive Tyrant: DASH ATTACK!");

        // Stretch before dashing
        transform.localScale = new Vector3(originalScale.x * 1.5f, originalScale.y * 0.7f, originalScale.z);

        // Get dash direction
        Vector2 dashDirection = (playerTransform.position - transform.position).normalized;
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

        Debug.Log("Hive Tyrant: Summoning Stinglets!");
        Instantiate(stingletPrefab, transform.position, Quaternion.identity);
        activeMinions++;
    }

    protected override void AdvanceToNextStage()
    {
        if (currentStage < 2)
        {
            currentStage = 2;
            Debug.Log("Hive Tyrant has entered Stage 2!");

            enemyMovement.moveSpeed *= enragedSpeedMultiplier;
            transform.localScale *= 1.2f; 
        }
    }

}
