using System.Collections;
using UnityEngine;

[RequireComponent(typeof(RangedEnemyAttack))]
public class VoidStalkerBoss : Boss
{
     [Header("STAGE 1: ENERGY ORBS")]
    [SerializeField] private GameObject homingOrbPrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float orbFireRate = 2f;
    [SerializeField] private float teleportCooldown = 5f;
    private float teleportTimer;

    [Header("STAGE 2: SPATIAL RIFT")]
    [SerializeField] private GameObject voidRiftPrefab;
    [SerializeField] private float riftExplosionDelay = 2f;
    [SerializeField] private float shieldDuration = 3f;

    [Header("STAGE 3: REALITY TEAR")]
    [SerializeField] private GameObject blackHolePrefab;
    [SerializeField] private GameObject spinningProjectilePrefab;
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
        base.Update();
        
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

    // === STAGE 1: Fire Homing Orbs ===
    private void FireHomingOrbs()
    {
        Instantiate(homingOrbPrefab, firePoint.position, Quaternion.identity);
        isAttacking = false;
    }

    // === STAGE 2: Teleport and Spawn Void Rift ===
    private void TeleportWithVoidRift()
    {
        Vector3 previousPosition = transform.position;
        Teleport();
        GameObject rift = Instantiate(voidRiftPrefab, previousPosition, Quaternion.identity);
        Destroy(rift, riftExplosionDelay);
        StartCoroutine(ActivateShield());
        isAttacking = false;
    }

    private IEnumerator ActivateShield()
    {
        // Activate shield logic
        yield return new WaitForSeconds(shieldDuration);
        // Deactivate shield
    }

    // === STAGE 3: Spawn Black Hole ===
    private void SpawnBlackHole()
    {
        GameObject blackHole = Instantiate(blackHolePrefab, transform.position, Quaternion.identity);
        Destroy(blackHole, blackHoleDuration);
        FireSpinningProjectiles();
        isAttacking = false;
    }

    private void FireSpinningProjectiles()
    {
        for (int i = 0; i < 8; i++)
        {
            Instantiate(spinningProjectilePrefab, transform.position, Quaternion.Euler(0, 0, i * 45));
        }
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
    }
}
