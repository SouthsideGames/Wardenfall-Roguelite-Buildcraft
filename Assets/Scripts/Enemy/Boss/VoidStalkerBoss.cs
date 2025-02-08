using System.Collections;
using UnityEngine;

[RequireComponent(typeof(RangedEnemyAttack))]
public class VoidStalkerBoss : Boss
{
     [Header("STAGE 1")]
    [SerializeField] private float teleportCooldown = 5f;
    private float teleportTimer;

    [Header("STAGE 2")]
    [SerializeField] private GameObject voidRiftPrefab;
    [SerializeField] private float riftExplosionDelay = 2f;

    [Header("STAGE 3")]
    [SerializeField] private GameObject blackHolePrefab;
    [SerializeField] private float blackHoleDuration = 5f;

    private bool isAttacking;
    private RangedEnemyAttack rangedEnemyAttack;

    private void Awake() 
    {
        rangedEnemyAttack = GetComponent<RangedEnemyAttack>();  
    }

    protected override void InitializeBoss()
    {
        base.InitializeBoss();
        teleportTimer = teleportCooldown;
    }

    protected override void Update()
    {
        UpdateHealthUI();
        ManageStates();

        ChangeDirections();

        DetectSurvivorBox();

        if (detectedBox != null && CanAttack())
        {
            AttackBox();
        }

        if (!hasSpawned || isAttacking) return;

        teleportTimer -= Time.deltaTime;

        if (teleportTimer <= 0)
        {
            Teleport();
            teleportTimer = teleportCooldown;
        }
    }

    protected override void ExecuteStageOne()
    {
        if (isAttacking) return;
        isAttacking = true;
        FireHomingOrbs();
    }

    protected override void ExecuteStageTwo()
    {
        if (isAttacking) return;
        isAttacking = true;
        TeleportWithVoidRift();
    }

    protected override void ExecuteStageThree()
    {
        if (isAttacking) return;
        isAttacking = true;
        SpawnBlackHole();
    }

    private void FireHomingOrbs()
    {
        rangedEnemyAttack.AutoAim();
        isAttacking = false;
    }

    // === STAGE 2: Teleport and Spawn Void Rift ===
    private void TeleportWithVoidRift()
    {
        Vector3 previousPosition = transform.position;
        Teleport();
        GameObject rift = Instantiate(voidRiftPrefab, previousPosition, Quaternion.identity);
        Destroy(rift, riftExplosionDelay);
        isAttacking = false;
    }

    // === STAGE 3: Spawn Black Hole ===
    private void SpawnBlackHole()
    {
        GameObject blackHole = Instantiate(blackHolePrefab, transform.position, Quaternion.identity);
        Destroy(blackHole, blackHoleDuration);
        isAttacking = false;
    }

    // === TELEPORTATION ===
    private void Teleport()
    {
        Vector3 randomPosition = new Vector3(
            Random.Range(-5f, 5f),
            Random.Range(-5f, 5f),
            transform.position.z
        );
        transform.position = randomPosition;

        ExecuteStage();
    }
}
