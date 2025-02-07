using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GravulonBoss : Boss
{

     [Header("STAGE 1 - Shockwave Slam")]
    [SerializeField] private float slamRange = 3f;
    [SerializeField] private int slamDamage = 25;
    [SerializeField] private float stunDuration = 1.5f;
    [SerializeField] private float slamCooldown = 6f;

    private EnemyMovement enemyMovement;
    private bool isSlamming;
    private float slamTimer;
    private Vector3 originalScale;

    protected override void InitializeBoss()
    {
        base.InitializeBoss();
        enemyMovement = GetComponent<EnemyMovement>();
        originalScale = transform.localScale;
        slamTimer = slamCooldown;
    }

    protected override void Update()
    {
        base.Update();

        if (!hasSpawned || isSlamming) return;

        // Reduce attack cooldown
        slamTimer -= Time.deltaTime;

        if (slamTimer <= 0)
        {
            ExecuteStage(); // Calls the correct attack (Stage 1)
            slamTimer = slamCooldown; // Reset cooldown timer
        }
        else
        {
            enemyMovement.FollowCurrentTarget();
        }
    }

    // === SINGLE-STAGE ATTACK: SHOCKWAVE SLAM ===
    protected override void ExecuteStageOne()
    {
        if (isSlamming) return;

        isSlamming = true;
        enemyMovement.DisableMovement(1.5f);
        Debug.Log("Gravulon: Preparing Slam!");

        // **1. Prepare for the Slam**
        transform.localScale = new Vector3(originalScale.x * 1.2f, originalScale.y * 0.8f, originalScale.z);

        // **2. Slam Down**
        Invoke(nameof(PerformSlam), 0.5f); // Delay for animation
    }

    private void PerformSlam()
    {
        transform.localScale = new Vector3(originalScale.x * 0.8f, originalScale.y * 1.2f, originalScale.z);
        Debug.Log("Gravulon: SHOCKWAVE SLAM!");
        DealShockwaveDamage();

        // **3. Reset size and allow movement again**
        Invoke(nameof(ResetAfterSlam), 0.3f);
    }

    private void ResetAfterSlam()
    {
        transform.localScale = originalScale;
        isSlamming = false;
        enemyMovement.EnableMovement();
    }

    private void DealShockwaveDamage()
    {
        Collider2D[] hitObjects = Physics2D.OverlapCircleAll(transform.position, slamRange);

        foreach (Collider2D hit in hitObjects)
        {
            if (hit.CompareTag("Player"))
            {
                character.TakeDamage(slamDamage);
                Debug.Log("Gravulon slammed the player!");
            }
            else if (hit.CompareTag("Enemy"))
            {
                Enemy enemy = hit.GetComponent<Enemy>();
                if (enemy != null)
                {
                    enemy.TakeDamage(slamDamage, false);
                    movement.DisableMovement(stunDuration);
                    Debug.Log($"Gravulon stunned enemy: {hit.name}");
                }
            }
            else if (hit.CompareTag("SurvivorBox"))
            {
                SurvivorBox box = hit.GetComponent<SurvivorBox>();
                if (box != null)
                {
                    box.TakeDamage(slamDamage);
                    Debug.Log("Gravulon damaged the Survivor Box!");
                }
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, slamRange);
    }
}
