using Unity.VisualScripting;
using UnityEngine;
using System.Collections;

[RequireComponent(typeof(EnemyMovement))]
[RequireComponent(typeof(RangedEnemyAttack))]
public class BramblethornBoss : Boss
{
    [Header("STAGE 1")]
    [SerializeField] private GameObject spikePrefab;
    [SerializeField] private float rootSlamRadius = 2.5f;
    [SerializeField] private int rootSlamDamage = 15;

    [Header("STAGE 2")]
    [SerializeField] private int thornBarrageCount = 5;
    [SerializeField] private float thornBarrageDelay = 0.2f;

    [Header("STAGE 3")]
    [SerializeField] private float chargeDuration = 1.5f;
    [SerializeField] private float chargeSpeedMultiplier = 3f;
    [SerializeField] private int chargeDamage = 20;

    private EnemyMovement enemyMovement;
    private bool isAttacking = false;
    private RangedEnemyAttack rangedEnemyAttack;
    private float normalSpeed;

    protected override void InitializeBoss()
    {
        base.InitializeBoss();
        enemyMovement = GetComponent<EnemyMovement>();
        rangedEnemyAttack = GetComponent<RangedEnemyAttack>();  

        rangedEnemyAttack.StorePlayer(character);
        normalSpeed = enemyMovement.moveSpeed;
    }

    protected override void Update()
    {
        base.Update();

        if (!hasSpawned || isAttacking) return;

        float distanceToPlayer = Vector2.Distance(transform.position, PlayerTransform.position);

        if (distanceToPlayer <= playerDetectionRadius)
            ExecuteStage();
        else
            enemyMovement.FollowCurrentTarget();
    }

    protected override void ExecuteStageOne() => RootSlam();
    protected override void ExecuteStageTwo() => ThornBarrage();
    protected override void ExecuteStageThree() => BristleCharge();

    private void RootSlam()
    {
        if (isAttacking) return;
        isAttacking = true;

        enemyMovement.DisableMovement(1f);
        Instantiate(spikePrefab, transform.position, Quaternion.identity);

        Collider2D[] hitObjects = Physics2D.OverlapCircleAll(transform.position, rootSlamRadius);
        foreach (Collider2D hit in hitObjects)
        {
            if (hit.CompareTag("Player"))
                CharacterManager.Instance.health.TakeDamage(rootSlamDamage);
        }

        Invoke(nameof(ResetAttack), 1f);
    }

    private IEnumerator ThornBarrage()
    {
        if (isAttacking) yield break;
        isAttacking = true;
        enemyMovement.DisableMovement(1.5f);

        for (int i = 0; i < thornBarrageCount; i++)
        {
            rangedEnemyAttack.AutoAim();
            yield return new WaitForSeconds(thornBarrageDelay);
        }

        ResetAttack();
    }

    private void BristleCharge()
    {
        if (isAttacking) return;
        isAttacking = true;

        enemyMovement.moveSpeed *= chargeSpeedMultiplier;
        Invoke(nameof(EndCharge), chargeDuration);
    }

    private void EndCharge()
    {
        enemyMovement.moveSpeed = normalSpeed;
        ResetAttack();
    }

    private void ResetAttack() => isAttacking = false;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isAttacking && collision.gameObject.CompareTag("Player"))
            CharacterManager.Instance.health.TakeDamage(chargeDamage);
    }
}