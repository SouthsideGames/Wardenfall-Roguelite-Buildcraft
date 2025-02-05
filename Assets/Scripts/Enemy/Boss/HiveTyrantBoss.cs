using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HiveTyrantBoss : Boss
{
    [Header("HIVE TYRANT")]
    [SerializeField] private float moveSpeed = 5f;
    
    [Header("Stage 1: Standard Behavior")]
    [SerializeField] private float dashSpeed = 10f;
    [SerializeField] private float dashCooldown = 3f;
    [SerializeField] private float spawnInterval = 5f;
    
    [Header("Stage 2: Aggressive Behavior")]
    [SerializeField] private float enragedDashCooldown = 2f;
    [SerializeField] private float enragedSpawnInterval = 3f;
    [SerializeField] private float enragedSpeedMultiplier = 1.3f; // Increases speed in Stage 2
    [SerializeField] private float stageTwoHealthThreshold = 0.5f; // 50% health

    [Header("Minions")]
    [SerializeField] private GameObject stingletPrefab;
    [SerializeField] private int maxMinions = 3;

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
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;

        dashTimer = dashCooldown;
        spawnTimer = spawnInterval;
    }

    protected override void Update()
    {
        base.Update();

        if (!hasSpawned || isDashing) return;

        // **Check if we should enter Stage 2**
        if (currentStage == 1 && (float)health / maxHealth <= stageTwoHealthThreshold)
        {
            AdvanceToNextStage();
        }

        // **Set cooldowns based on stage**
        float currentDashCooldown = (currentStage == 2) ? enragedDashCooldown : dashCooldown;
        float currentSpawnInterval = (currentStage == 2) ? enragedSpawnInterval : spawnInterval;

        // **Cooldown Timers**
        dashTimer -= Time.deltaTime;
        spawnTimer -= Time.deltaTime;

        if (dashTimer <= 0)
        {
            ExecuteStage();
            dashTimer = currentDashCooldown; // Reset dash cooldown
        }
        else if (spawnTimer <= 0 && activeMinions < maxMinions)
        {
            SpawnMinion();
            spawnTimer = currentSpawnInterval; // Reset spawn cooldown
        }
        else
        {
            enemyMovement.FollowCurrentTarget();
        }
    }

    // === STAGE 1 & 2 ATTACK: DASH ATTACK ===
    protected override void ExecuteStage()
    {
        if (!isDashing)
        {
            isDashing = true;
            enemyMovement.DisableMovement(0.6f);

            // **1. Prepare for Dash Animation**
            transform.localScale = new Vector3(originalScale.x * 1.5f, originalScale.y * 0.7f, originalScale.z);

            Invoke(nameof(PerformDash), 0.2f);
        }
    }

    private void PerformDash()
    {
        Vector2 dashDirection = (playerTransform.position - transform.position).normalized;
        transform.position += (Vector3)(dashDirection * 3f);

        // **2. Recovery After Dash**
        transform.localScale = new Vector3(originalScale.x * 0.7f, originalScale.y * 1.3f, originalScale.z);
        Invoke(nameof(ResetAfterDash), 0.1f);
    }

    private void ResetAfterDash()
    {
        transform.localScale = originalScale;
        isDashing = false;
    }

    // === MINION SPAWNING (USED IN BOTH STAGES) ===
    private void SpawnMinion()
    {
        Instantiate(stingletPrefab, transform.position, Quaternion.identity);
        activeMinions++;
    }

    // === TRANSITION TO STAGE 2 ===
    protected override void AdvanceToNextStage()
    {
        if (currentStage < 2)
        {
            currentStage = 2;
            Debug.Log("Hive Tyrant has entered Stage 2!");

            enemyMovement.moveSpeed *= enragedSpeedMultiplier;
            transform.localScale *= 1.2f; // Slightly grow in size for intimidation
        }
    }


}
