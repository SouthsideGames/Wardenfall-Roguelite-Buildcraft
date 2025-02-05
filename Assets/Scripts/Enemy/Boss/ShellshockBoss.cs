using UnityEngine;

[RequireComponent(typeof(EnemyMovement))]
public class ShellshockBoss : Boss
{
    [Header("BOSS MOVEMENT SETTINGS")]
    [SerializeField] private float moveDuration = 3f;  // How long the boss moves before stopping
    [SerializeField] private float stopDuration = 1.5f; // How long the boss stops before attacking
    [SerializeField] private float moveRange = 5f; // Random movement range

    [Header("BOSS ATTACK SETTINGS")]
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

    protected override void Update()
    {
        base.Update();

        if (!hasSpawned || isAttacking) return;

        if (isMoving)
        {
            MoveToTarget();
        }

        if(isAttacking)
            FireHomingBullet();
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
        isAttacking = true;
        enemyMovement.DisableMovement(stopDuration);
        Invoke(nameof(ResumeMovement), stopDuration);
    }

    private void FireHomingBullet() => Instantiate(homingBulletPrefab, firePoint.position, Quaternion.identity);

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
