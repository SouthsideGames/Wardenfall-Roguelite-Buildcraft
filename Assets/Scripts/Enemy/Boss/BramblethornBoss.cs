using UnityEngine;

[RequireComponent(typeof(EnemyMovement))]
public class BramblethornBoss : Boss
{
    [Header("MOVEMENT SETTINGS")]
    [SerializeField] private float normalSpeed = 1.5f;
    [SerializeField] private float chargeSpeedMultiplier = 3f;
    
    [Header("ATTACK SETTINGS")]
    [SerializeField] private GameObject thornProjectilePrefab;
    [SerializeField] private GameObject spikePrefab;
    [SerializeField] private Transform firePoint;
    
    [Header("PHASE SETTINGS")]
    [SerializeField] private float thornBarrageSpeed = 6f;
    [SerializeField] private float chargeDuration = 1.5f;

    private EnemyMovement enemyMovement;
    private bool isAttacking = false;

    protected override void InitializeBoss()
    {
        base.InitializeBoss();
        enemyMovement = GetComponent<EnemyMovement>();
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
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, 2.5f);
        foreach (Collider2D hit in hitEnemies)
        {
            if (hit.CompareTag("Player"))
                hit.GetComponent<CharacterManager>().TakeDamage(15);
        }

        Invoke(nameof(ResetAttack), 1f);
    }

    private void ThornBarrage()
    {
        if (isAttacking) return;
        isAttacking = true;
        enemyMovement.DisableMovement(1.5f);
        Debug.Log("Bramblethorn: THORN BARRAGE!");

        for (int i = 0; i < 5; i++)
        {
            GameObject thorn = Instantiate(thornProjectilePrefab, firePoint.position, Quaternion.identity);
            thorn.GetComponent<Rigidbody2D>().linearVelocity = (playerTransform.position - transform.position).normalized * thornBarrageSpeed;
        }

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