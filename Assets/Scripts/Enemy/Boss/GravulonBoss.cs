using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GravulonBoss : Boss
{
    [Header("STAGE 1")]
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

        slamTimer -= Time.deltaTime;

        if (slamTimer <= 0)
        {
            ExecuteStage();
            slamTimer = slamCooldown;
        }
        else
            enemyMovement.FollowCurrentTarget();
    }

    protected override void ExecuteStage()
    {

        switch (stageToExecute)
        {
            case 1:
                ExecuteStageOne();
                break;
            case 2:
                ExecuteStageTwo();
                break;
            default:
                Debug.LogWarning("Invalid Stage Selected for Gravulon!");
                break;
        }
    }

    protected override void ExecuteStageOne()
    {
        if (isSlamming) return;

        isSlamming = true;
        enemyMovement.DisableMovement(1.5f);

        transform.localScale = new Vector3(originalScale.x * 1.2f, originalScale.y * 0.8f, originalScale.z);

        Invoke(nameof(PerformSlam), 0.5f);
    }

    private void PerformSlam()
    {
        transform.localScale = new Vector3(originalScale.x * 0.8f, originalScale.y * 1.2f, originalScale.z);
        DealShockwaveDamage();

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
                character.TakeDamage(slamDamage);
            else if (hit.CompareTag("Enemy"))
            {
                Enemy enemy = hit.GetComponent<Enemy>();
                if (enemy != null)
                {
                    enemy.TakeDamage(slamDamage, false);
                    enemy.GetComponent<EnemyMovement>().DisableMovement(stunDuration);
                }
            }
        }
    }

    protected override void ExecuteStageTwo()
    {
        if (isSlamming) return;

        isSlamming = true;
        enemyMovement.DisableMovement(2f);

        transform.localScale = new Vector3(originalScale.x * 1.1f, originalScale.y * 0.9f, originalScale.z);
        Invoke(nameof(PerformEarthquake), 0.5f);
    }

    private void PerformEarthquake()
    {
        transform.localScale = originalScale;

        Collider2D[] hitObjects = Physics2D.OverlapCircleAll(transform.position, slamRange * 1.5f);
        foreach (Collider2D hit in hitObjects)
        {
            if (hit.CompareTag("Player"))
                character.TakeDamage(slamDamage * 2);
            else if (hit.CompareTag("Enemy"))
            {
                Enemy enemy = hit.GetComponent<Enemy>();
                if (enemy != null)
                {
                    enemy.TakeDamage(slamDamage, false);
                    enemy.GetComponent<EnemyMovement>().DisableMovement(stunDuration * 1.5f);
                }
            }
        }

        isSlamming = false;
        enemyMovement.EnableMovement();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, slamRange);
    }
}
