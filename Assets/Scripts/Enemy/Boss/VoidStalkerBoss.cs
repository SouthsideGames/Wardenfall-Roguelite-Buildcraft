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
    [SerializeField] private float voidRiftDuration = 2f;

    [Header("STAGE 3")]
    [SerializeField] private GameObject blackHolePrefab;
    [SerializeField] private float blackHoleDuration = 5f;

    private bool isAttacking;
    private RangedEnemyAttack rangedEnemyAttack;

    protected override void InitializeBoss()
    {
        base.InitializeBoss();
        teleportTimer = teleportCooldown;
        rangedEnemyAttack = GetComponent<RangedEnemyAttack>();
        rangedEnemyAttack.StorePlayer(character);
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

    protected override void ExecuteStageOne() => FireHomingOrbs();
    protected override void ExecuteStageTwo() => TeleportWithVoidRift();
    protected override void ExecuteStageThree() => SpawnBlackHole();

    private void FireHomingOrbs()
    {
        if (isAttacking) return;
        isAttacking = true;
        rangedEnemyAttack.AutoAim();
        Invoke(nameof(ResetAttack), 1f);
    }


    private void TeleportWithVoidRift()
    {
        if (isAttacking) return;
        isAttacking = true;

        Vector3 previousPosition = transform.position;
        Teleport();

        GameObject rift = Instantiate(voidRiftPrefab, previousPosition, Quaternion.identity);
        Destroy(rift, voidRiftDuration);

        Invoke(nameof(ResetAttack), 1.5f);
    }

    private void SpawnBlackHole()
    {
        if (isAttacking) return;
        isAttacking = true;

        GameObject blackHole = Instantiate(blackHolePrefab, transform.position, Quaternion.identity);
        Destroy(blackHole, blackHoleDuration);

        Invoke(nameof(ResetAttack), 2f);
    }

    private void Teleport()
    {
        Debug.Log("Void Stalker: TELEPORT!");
        Vector3 randomPosition = GetRandomTeleportPosition();
        transform.position = randomPosition;

        ExecuteStage(); 
    }

    private Vector3 GetRandomTeleportPosition()
    {
        float teleportRange = 5f;
        return new Vector3(
            Random.Range(-teleportRange, teleportRange),
            Random.Range(-teleportRange, teleportRange),
            transform.position.z
        );
    }

    private void ResetAttack() => isAttacking = false;
}
