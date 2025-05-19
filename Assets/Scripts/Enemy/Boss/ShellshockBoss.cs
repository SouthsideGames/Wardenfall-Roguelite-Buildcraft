
using UnityEngine;
using System.Collections;

[RequireComponent(typeof(EnemyMovement))]
[RequireComponent(typeof(RangedEnemyAttack))]
public class ShellshockBoss : Boss
{
    [Header("MOVEMENT SETTINGS")]
    [SerializeField] private float stopDuration = 1.5f;
    [SerializeField] private float moveRange = 5f;

    private EnemyMovement enemyMovement;
    private RangedEnemyAttack rangedAttack;
    private Vector2 randomTargetPosition;
    private bool isMoving = true;
    private bool isAttacking = false;

    protected override void InitializeBoss()
    {
        base.InitializeBoss();
        enemyMovement = GetComponent<EnemyMovement>();
        rangedAttack = GetComponent<RangedEnemyAttack>();

        rangedAttack.StorePlayer(character);
        PickNewRandomTarget();
    }

    protected override void Update()
    {
        base.Update();

        if (!hasSpawned || isAttacking) return;

        if (isMoving)
            MoveToTarget();
    }

    private void MoveToTarget()
    {
        enemyMovement.SetTargetPosition(randomTargetPosition);

        if (Vector2.Distance(transform.position, randomTargetPosition) < 0.2f)
            StopAndAttack();
    }

    private void StopAndAttack()
    {
        isMoving = false;
        isAttacking = true;
        enemyMovement.DisableMovement(stopDuration);

        Invoke(nameof(StartAutoAim), stopDuration * 0.5f);
        Invoke(nameof(ResumeMovement), stopDuration);
    }

    private void StartAutoAim() => rangedAttack.AutoAim();

    private void ResumeMovement()
    {
        isMoving = true;
        isAttacking = false;
        PickNewRandomTarget();
    }

    private void PickNewRandomTarget()
    {
        Vector2 bossPosition = transform.position;
        randomTargetPosition = new Vector2(
            bossPosition.x + Random.Range(-moveRange, moveRange),
            bossPosition.y + Random.Range(-moveRange, moveRange)
        );
    }
}
