using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TwinfangBoss : Boss
{
    [Header("STAGE 1")]
    [SerializeField] private float lungeSpeed = 8f;  
    [SerializeField] private float retreatSpeed = 6f; 
    [SerializeField] private float attackRange = 3f;

    private EnemyMovement enemyMovement;
    private bool isAttacking;
    private float attackCooldownTimer;
    private Vector3 originalScale;


    protected override void InitializeBoss()
    {
        base.InitializeBoss();
        enemyMovement = GetComponent<EnemyMovement>();
        originalScale = transform.localScale;
        attackCooldownTimer = attackCooldown;
    }

    protected override void Update()
    {
        base.Update();

        if (!hasSpawned || isAttacking) return;

        attackCooldownTimer -= Time.deltaTime;

        if (!isAttacking) enemyMovement.FollowCurrentTarget();

        if (Vector2.Distance(transform.position, playerTransform.position) <= attackRange && attackCooldownTimer <= 0)
        {
            ExecuteStage();
        }
    }

    // === SINGLE-STAGE ATTACK: LUNGE ATTACK ===
    protected override void ExecuteStage()
    {
        if (isAttacking) return;

        isAttacking = true;
        enemyMovement.DisableMovement(0.6f); 

        transform.localScale = new Vector3(originalScale.x * 1.5f, originalScale.y * 0.7f, originalScale.z);
        Invoke(nameof(PerformLunge), 0.2f);
    }

    private void PerformLunge()
    {
        Vector2 attackDirection = (playerTransform.position - transform.position).normalized;
        transform.position += (Vector3)(attackDirection * attackRange);

        transform.localScale = new Vector3(originalScale.x * 0.7f, originalScale.y * 1.3f, originalScale.z);
        Invoke(nameof(PerformRetreat), 0.1f);
    }

    private void PerformRetreat()
    {
        Vector2 attackDirection = (playerTransform.position - transform.position).normalized;
        transform.position -= (Vector3)(attackDirection * (attackRange * 0.75f));

        transform.localScale = originalScale;
        attackCooldownTimer = attackCooldown;
        isAttacking = false;
    }
}