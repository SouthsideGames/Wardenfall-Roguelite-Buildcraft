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

    private int currentStage = 1;
    private bool isAttacking;
    private Vector3 originalScale;
    private RangedEnemyAttack rangedAttack;

    protected override void InitializeBoss()
    {
        base.InitializeBoss();
        originalScale = transform.localScale;
        rangedAttack = GetComponent<RangedEnemyAttack>();
        StartCoroutine(AttackLoop());
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

            case BossState.Transitioning:
                ManageTransition();
                break;
        }
    }

    private IEnumerator AttackLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(attackCooldown);
            if (bossState == BossState.Idle) StartAttackingState();
        }
    }

    // === Movement for Later Stages ===
    private void MoveErratically()
    {
        transform.position += new Vector3(Mathf.Sin(Time.time * 3), Mathf.Cos(Time.time * 3), 0) * Time.deltaTime * erraticMoveSpeed;
    }

    // === Transition to Next Stage ===
    private void ManageTransition()
    {
        StartCoroutine(TransitionSequence());
    }

    private IEnumerator TransitionSequence()
    {
        // Stop movement and grow larger
        bossState = BossState.Transitioning;
        movement.DisableMovement(1.5f);
        transform.localScale *= 1.2f;
        yield return new WaitForSeconds(1.5f);

        currentStage++;
        Debug.Log($"Eyefang Prime advanced to Stage {currentStage}!");

        // Reset scale slightly for balance
        transform.localScale = originalScale * (1f + (currentStage * 0.2f));
        movement.EnableMovement();

        SetIdleState();
    }

    // === ATTACKS ===
    protected override void ExecuteAttack()
    {
        isAttacking = true;

        switch (currentStage)
        {
            case 1:
                StartCoroutine(FireProjectileAttack());
                break;
            case 2:
                StartCoroutine(RadialBeamAttack());
                break;
            case 3:
                StartCoroutine(KnockbackBurst());
                break;
        }
    }

    private IEnumerator FireProjectileAttack()
    {
        anim.Play("Shoot");
        yield return new WaitForSeconds(0.2f);

        rangedAttack.AutoAim(); // Fire a projectile at the player

        SetIdleState();
    }

    private IEnumerator RadialBeamAttack()
    {
        anim.Play("Expand");
        yield return new WaitForSeconds(0.5f);

        float angleStep = 360f / beamCount;
        for (int i = 0; i < beamCount; i++)
        {
            float angle = i * angleStep;
            Instantiate(beamPrefab, transform.position, Quaternion.Euler(0, 0, angle));
        }

        SetIdleState();
    }

    private IEnumerator KnockbackBurst()
    {
        anim.Play("Deflate");
        yield return new WaitForSeconds(0.3f);

        Collider2D[] hitObjects = Physics2D.OverlapCircleAll(transform.position, burstRadius);
        foreach (Collider2D hit in hitObjects)
        {
            if (hit.CompareTag("Player"))
            {
                character.TakeDamage(burstDamage);
            }
        }

        anim.Play("Inflate");
        yield return new WaitForSeconds(0.3f);
        SetIdleState();
    }
}
