using System.Collections;
using UnityEngine;

[RequireComponent(typeof(RangedEnemyAttack))]
public class EyeFangBoss : Boss
{
    [Header("Eyefang Prime Settings")]
    [SerializeField] private float erraticMoveSpeed = 3f;
    [SerializeField] private float attackCooldown = 3f;

    [Header("Stage 2: Radial Beams")]
    [SerializeField] private GameObject beamPrefab;
    [SerializeField] private int beamCount = 8;

    [Header("Stage 3: Knockback Burst")]
    [SerializeField] private float burstRadius = 3f;
    [SerializeField] private int knockbackForce = 5;
    [SerializeField] private int burstDamage = 20;

    private bool isAttacking;
    private Vector3 originalScale;
    private RangedEnemyAttack rangedAttack;

    protected override void InitializeBoss()
    {
        base.InitializeBoss();
        originalScale = transform.localScale;
        rangedAttack = GetComponent<RangedEnemyAttack>();
    }

    protected override void ManageStates()
    {
        switch (bossState)
        {
            case BossState.Idle:
                ManageIdleState();
                break;

            case BossState.Moving:
                if (currentStage >= 2) MoveErratically();
                else ManageMovingState();
                break;

            case BossState.Attacking:
                ExecuteAttack();
                break;
        }
    }

    // === Movement for Later Stages ===
    private void MoveErratically()
    {
        transform.position += new Vector3(Mathf.Sin(Time.time * 3), Mathf.Cos(Time.time * 3), 0) * Time.deltaTime * erraticMoveSpeed;
    }


    // === ATTACKS ===
    protected override void ExecuteStageOne()
    {
        if (!isAttacking)
        {
            isAttacking = true;
            FireProjectileAttack();
        }
    }

    protected override void ExecuteStageTwo()
    {
        if (!isAttacking)
        {
            isAttacking = true;
            RadialBeamAttack();
        }
    }

    protected override void ExecuteStageThree()
    {
        if (!isAttacking)
        {
            isAttacking = true;
            KnockbackBurst();
        }
    }

    private void FireProjectileAttack()
    {
        anim.Play("Shoot");
        rangedAttack.AutoAim(); // Fire a projectile at the player
        isAttacking = false;
    }

    private void RadialBeamAttack()
    {
        anim.Play("Expand");

        float angleStep = 360f / beamCount;
        for (int i = 0; i < beamCount; i++)
        {
            float angle = i * angleStep;
            Instantiate(beamPrefab, transform.position, Quaternion.Euler(0, 0, angle));
        }

        isAttacking = false;
    }

    private void KnockbackBurst()
    {
        anim.Play("Deflate");

        Collider2D[] hitObjects = Physics2D.OverlapCircleAll(transform.position, burstRadius);
        foreach (Collider2D hit in hitObjects)
        {
            if (hit.CompareTag("Player"))
            {
                character.TakeDamage(burstDamage);
            }
        }

        anim.Play("Inflate");
        isAttacking = false;
    }
}
