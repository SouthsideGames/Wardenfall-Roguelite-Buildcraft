using UnityEditor.EditorTools;
using UnityEngine;

[RequireComponent(typeof(EnemyMovement))]
public class ShellshockBoss : Boss
{
     [Header("MOVEMENT SETTINGS")]
    [SerializeField] private float stopDuration = 1.5f;
    [SerializeField] private float moveRange = 5f;

    [Header("ATTACK SETTINGS")]
    [SerializeField] private GameObject homingBulletPrefab;
    [SerializeField] private Transform firePoint;

    private EnemyMovement enemyMovement;
    private Vector2 randomTargetPosition;
    private bool isMoving = true;
    private bool isAttacking = false;

    protected override void InitializeBoss()
    {
        base.InitializeBoss();
        enemyMovement = GetComponent<EnemyMovement>();
        PickNewRandomTarget();
    }

    protected override void ExecuteStageOne()
    {
        FireHomingBullet();
    }

    protected override void Update()
    {
        base.Update();

        if (!hasSpawned || isAttacking) return;

        if (isMoving)
        {
            MoveToTarget();
        }
    }

    private void MoveToTarget()
    {
        enemyMovement.SetTargetPosition(randomTargetPosition);

        if (Vector2.Distance(transform.position, randomTargetPosition) < 0.2f)
        {
            StopAndAttack();
        }
    }

    private void StopAndAttack()
    {
        isMoving = false;
        enemyMovement.DisableMovement(stopDuration);
        Invoke(nameof(ResumeMovement), stopDuration);
        ExecuteStageOne();
    }

    private void FireHomingBullet()
    {
        Debug.Log("Shellshock: Firing Homing Bullet!");
        Instantiate(homingBulletPrefab, firePoint.position, Quaternion.identity);
    }

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
