using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HiveTyrantBoss : Boss
{   
    [Header("STAGE 1")]
    [SerializeField] private float dashSpeed = 10f;
    [SerializeField] private float dashCooldown = 3f;
    [SerializeField] private float enragedDashCooldown = 2f;
    [SerializeField] private float enragedSpawnInterval = 3f;
    [SerializeField] private float enragedSpeedMultiplier = 1.3f;
    [SerializeField] private float stageTwoHealthThreshold = 0.5f; 

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

        if (currentStage == 1 && (float)health / maxHealth <= stageTwoHealthThreshold)
        {
            AdvanceToNextStage();
        }

        float currentDashCooldown = (currentStage == 2) ? enragedDashCooldown : dashCooldown;
        float currentSpawnInterval = (currentStage == 2) ? enragedSpawnInterval : spawnInterval;

        dashTimer -= Time.deltaTime;
        spawnTimer -= Time.deltaTime;

        if (dashTimer <= 0)
        {
            ExecuteStage();
            dashTimer = currentDashCooldown;
        }
        else if (spawnTimer <= 0 && activeMinions < maxMinions)
        {
            SpawnMinion();
            spawnTimer = currentSpawnInterval;
        }
        else
        {
            enemyMovement.FollowCurrentTarget();
        }
    }

    protected override void ExecuteStage()
    {
        if (!isDashing)
        {
            isDashing = true;
            enemyMovement.DisableMovement(0.6f);

            transform.localScale = new Vector3(originalScale.x * 1.5f, originalScale.y * 0.7f, originalScale.z);

            Invoke(nameof(PerformDash), 0.2f);
        }
    }

    private void PerformDash()
    {
        Vector2 dashDirection = (playerTransform.position - transform.position).normalized;
        transform.position += (Vector3)(dashDirection * 3f);

        transform.localScale = new Vector3(originalScale.x * 0.7f, originalScale.y * 1.3f, originalScale.z);
        Invoke(nameof(ResetAfterDash), 0.1f);
    }

    private void ResetAfterDash()
    {
        transform.localScale = originalScale;
        isDashing = false;
    }

    private void SpawnMinion()
    {
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
