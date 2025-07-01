using UnityEngine;
using System.Collections;

[RequireComponent(typeof(EnemyMovement))]
[RequireComponent(typeof(RangedEnemyAttack))]
[RequireComponent(typeof(Rigidbody2D))]
public class ShellshockBoss : Boss
{
    [Header("MOVEMENT SETTINGS")]
    [SerializeField] private float stopDuration = 1.5f;
    [SerializeField] private float moveRange = 5f;
    
    private EnemyMovement enemyMovement;
    private RangedEnemyAttack rangedAttack;
    private Rigidbody2D rb;
    private Vector2 randomTargetPosition;
    private bool isAttacking = false;

    protected override void InitializeBoss()
    {
        base.InitializeBoss();
        enemyMovement = GetComponent<EnemyMovement>();
        rangedAttack = GetComponent<RangedEnemyAttack>();
        rb = GetComponent<Rigidbody2D>();
        
        rangedAttack.StorePlayer(character);
        PickNewRandomTarget();
    }

    protected override void Update()
    {
        base.Update();

        if (!hasSpawned || isAttacking) return;

        enemyMovement.MoveToTargetPosition();
        if (Vector2.Distance(transform.position, randomTargetPosition) < 0.2f)
            StopAndAttack();
    }

    private void StopAndAttack()
    {
        isAttacking = true;
        enemyMovement.DisableMovement(stopDuration);

        Invoke(nameof(StartAutoAim), stopDuration * 0.5f);
        Invoke(nameof(ResumeMovement), stopDuration);
    }

    private void StartAutoAim() => rangedAttack.AutoAim();

    private void ResumeMovement()
    {
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
        enemyMovement.SetTargetPosition(randomTargetPosition);
    }

    protected override void ExecuteStageOne()
    {
        StartCoroutine(SpiralShotStage());
    }

    private IEnumerator SpiralShotStage()
    {
        isAttacking = true;
        enemyMovement.canMove = false;
        enemyMovement.StopMoving();

        LeanTween.rotateZ(gameObject, transform.eulerAngles.z + 360f, 2f).setEaseLinear();

        float angle = 0f;
        int shots = 6;
        for (int i = 0; i < shots; i++)
        {
            Vector2 dir = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
            rangedAttack.FireCustomDirection(dir.normalized, 3f);
            angle += 60f;
            yield return new WaitForSeconds(0.15f);
        }

        yield return new WaitForSeconds(0.5f);
        enemyMovement.canMove = true;
        isAttacking = false;
    }

    protected override void ExecuteStageTwo()
    {
        StartCoroutine(ShellChargeStage());
    }

    private IEnumerator ShellChargeStage()
    {
        isAttacking = true;
        enemyMovement.canMove = false;

        LeanTween.scale(gameObject, Vector3.one * 0.7f, 0.2f).setEaseInOutQuad();
        yield return new WaitForSeconds(0.2f);

        Vector2 chargeDirection = (PlayerTransform.position - transform.position).normalized;
        rb.AddForce(chargeDirection * 15f, ForceMode2D.Impulse);

        yield return new WaitForSeconds(1.5f);

        LeanTween.scale(gameObject, Vector3.one, 0.2f).setEaseInOutQuad();
        enemyMovement.canMove = true;
        isAttacking = false;
    }

    protected override void ExecuteStageThree()
    {
        StartCoroutine(ShellstormStage());
    }

    private IEnumerator ShellstormStage()
    {
        isAttacking = true;

        enemyMovement.canMove = true;
        enemyMovement.chasePlayer = true;
        enemyMovement.advanceThenStop = true;
        enemyMovement.stopDistance = 6f;

        LeanTween.scale(gameObject, transform.localScale * 1.3f, 0.2f).setEasePunch();
        yield return new WaitForSeconds(0.2f);

        for (int i = 0; i < 10; i++)
        {
            rangedAttack.AutoAim();
            yield return new WaitForSeconds(0.1f);
        }

        isAttacking = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isAttacking && collision.gameObject.CompareTag("Player"))
        {
            CharacterManager.Instance.health.TakeDamage(20);
        }
    }
}
