using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(EnemyMovement))]
[RequireComponent(typeof(RangedEnemyAttack))]
public class BramblethornBoss : Boss
{
    [Header("STAGE 1")]
    [SerializeField] private GameObject spikePrefab;
    [SerializeField] private Transform firePoint;

    [Header("STAGE 2")]
    [SerializeField] private float chargeDuration = 1.5f;

    [Header("STAGE 3")]
    [SerializeField] private float normalSpeed = 1.5f;
    [SerializeField] private float chargeSpeedMultiplier = 3f;

    private EnemyMovement enemyMovement;
    private bool isAttacking = false;
    private RangedEnemyAttack rangedEnemyAttack;

    protected override void InitializeBoss()
    {
        base.InitializeBoss();
        enemyMovement = GetComponent<EnemyMovement>();
        rangedEnemyAttack = GetComponent<RangedEnemyAttack>();  
    }

    protected override void Update()
    {
        base.Update();
        
        if (!hasSpawned || isAttacking) return;

        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);

        if (distanceToPlayer <= playerDetectionRadius)
        {
            ExecuteStage();
        }
        else
        {
            enemyMovement.FollowCurrentTarget();
        }
    }

    protected override void ExecuteStageOne()
    {
        RootSlam();
    }

    protected override void ExecuteStageTwo()
    {
        ThornBarrage();
    }

    protected override void ExecuteStageThree()
    {
        BristleCharge();
    }

    private void RootSlam()
    {
        if (isAttacking) return;
        isAttacking = true;

        enemyMovement.DisableMovement(1f);
        Debug.Log("Bramblethorn: ROOT SLAM!");

        Instantiate(spikePrefab, transform.position, Quaternion.identity);

        Invoke(nameof(ResetAttack), 1f);
    }

    private void ThornBarrage()
    {
        if (isAttacking) return;
        isAttacking = true;
        enemyMovement.DisableMovement(1.5f);
        Debug.Log("Bramblethorn: THORN BARRAGE!");

        for (int i = 0; i < 5; i++)
            rangedEnemyAttack.AutoAim();

        Invoke(nameof(ResetAttack), 1.5f);
    }

    private void BristleCharge()
    {
        if (isAttacking) return;
        isAttacking = true;
        Debug.Log("Bramblethorn: BRISTLE CHARGE!");

        enemyMovement.moveSpeed *= chargeSpeedMultiplier;
        Invoke(nameof(EndCharge), chargeDuration);
    }

    private void EndCharge()
    {
        enemyMovement.moveSpeed = normalSpeed;
        ResetAttack();
    }

    private void ResetAttack()
    {
        isAttacking = false;
    }
}