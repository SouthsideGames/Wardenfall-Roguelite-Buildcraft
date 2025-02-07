using System.Collections;
using UnityEngine;

[RequireComponent(typeof(RangedEnemyAttack))]
public class EyeFangBoss : Boss
{
     [Header("STAGE 1 - Normal Attack")]
    [SerializeField] private float erraticMoveSpeed = 3f;

    [Header("STAGE 2 - Radial Beams")]
    [SerializeField] private GameObject beamPrefab;
    [SerializeField] private int beamCount = 8;

    [Header("STAGE 3 - Knockback Burst")]
    [SerializeField] private float burstRadius = 3f;
    [SerializeField] private int burstDamage = 20;

    private RangedEnemyAttack rangedAttack;

    protected override void InitializeBoss()
    {
        base.InitializeBoss();
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
                ExecuteStage(); 
                break;
        }
    }

    // === MOVEMENT ===
    private void MoveErratically()
    {
        transform.position += new Vector3(
            Mathf.Sin(Time.time * 3), 
            Mathf.Cos(Time.time * 3), 
            0) * Time.deltaTime * erraticMoveSpeed;
    }

    // === STAGE EXECUTION SYSTEM ===
    protected override void ExecuteStageOne()
    {
        FireProjectileAttack();
    }

    protected override void ExecuteStageTwo()
    {
        RadialBeamAttack();
    }

    protected override void ExecuteStageThree()
    {
        KnockbackBurst();
    }

    // === ATTACKS ===
    private void FireProjectileAttack()
    {
        anim.Play("Shoot");
        rangedAttack.AutoAim();
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
    }
}
