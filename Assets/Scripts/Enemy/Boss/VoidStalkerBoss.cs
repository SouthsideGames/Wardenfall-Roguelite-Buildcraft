using System.Collections;
using UnityEngine;

[RequireComponent(typeof(RangedEnemyAttack))]
public class VoidStalkerBoss : Boss
{
    [Header("STAGE 1")]
    [SerializeField] private float teleportCooldown = 5f;

    [Header("STAGE 2")]
    [SerializeField] private GameObject voidRiftPrefab;
    [SerializeField] private float voidRiftDuration = 2f;

    [Header("STAGE 3")]
    [SerializeField] private GameObject blackHolePrefab;
    [SerializeField] private float blackHoleDuration = 5f;

    private bool isAttacking;
    private RangedEnemyAttack rangedEnemyAttack;
    private EnemyMovement enemyMovement;
    private float teleportTimer;

    protected override void InitializeBoss()
    {
        base.InitializeBoss();
        teleportTimer = teleportCooldown;
        rangedEnemyAttack = GetComponent<RangedEnemyAttack>();
        rangedEnemyAttack.StorePlayer(character);
        enemyMovement = GetComponent<EnemyMovement>();
    }

    protected override void Update()
    {
        base.Update();

        if (!hasSpawned || isAttacking) return;

        teleportTimer -= Time.deltaTime;

        if (teleportTimer <= 0)
        {
            StartCoroutine(TeleportRoutine());
            teleportTimer = teleportCooldown;
        }
        else
        {
            enemyMovement.FollowCurrentTarget();
        }
    }

    private float GetHealthPercent()
    {
        return (float)health / maxHealth;
    }

    private IEnumerator TeleportRoutine()
    {
        isAttacking = true;

        LeanTween.color(gameObject, Color.magenta, 0.2f).setLoopPingPong(2);
        LeanTween.scale(gameObject, transform.localScale * 1.2f, 0.2f).setEasePunch();

        yield return new WaitForSeconds(0.3f);

        Teleport();

        yield return new WaitForSeconds(0.2f);

        ExecuteStage();
    }

    private void Teleport()
    {
        Vector3 newPosition = GetRandomTeleportPosition();
        transform.position = newPosition;
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

    protected override void ExecuteStage()
    {
        if (GetHealthPercent() > 0.66f)
            ExecuteStageOne();
        else if (GetHealthPercent() > 0.33f)
            ExecuteStageTwo();
        else
            ExecuteStageThree();
    }

    protected override void ExecuteStageOne()
    {
        StartCoroutine(FireHomingOrbs());
    }

    private IEnumerator FireHomingOrbs()
    {
        LeanTween.color(gameObject, Color.cyan, 0.2f).setLoopPingPong(2);
        LeanTween.scale(gameObject, transform.localScale * 1.1f, 0.2f).setEasePunch();

        yield return new WaitForSeconds(0.2f);

        rangedEnemyAttack.AutoAim();

        yield return new WaitForSeconds(1f);
        isAttacking = false;
    }

    protected override void ExecuteStageTwo()
    {
        StartCoroutine(TeleportWithVoidRift());
    }

    private IEnumerator TeleportWithVoidRift()
    {
        Vector3 previousPosition = transform.position;

        LeanTween.color(gameObject, Color.black, 0.2f).setLoopPingPong(3);
        LeanTween.scale(gameObject, transform.localScale * 1.2f, 0.2f).setEasePunch();

        yield return new WaitForSeconds(0.3f);

        Teleport();

        GameObject rift = Instantiate(voidRiftPrefab, previousPosition, Quaternion.identity);
        Destroy(rift, voidRiftDuration);

        yield return new WaitForSeconds(1.5f);
        isAttacking = false;
    }

    protected override void ExecuteStageThree()
    {
        StartCoroutine(SpawnBlackHole());
    }

    private IEnumerator SpawnBlackHole()
    {
        LeanTween.color(gameObject, Color.black, 0.2f).setLoopPingPong(4);
        LeanTween.scale(gameObject, transform.localScale * 1.3f, 0.2f).setEasePunch();

        yield return new WaitForSeconds(0.3f);

        GameObject blackHole = Instantiate(blackHolePrefab, transform.position, Quaternion.identity);
        Destroy(blackHole, blackHoleDuration);

        yield return new WaitForSeconds(2f);
        isAttacking = false;
    }
}
