using UnityEngine;
using System.Collections;
using MoreMountains.Feedbacks;

[RequireComponent(typeof(EnemyMovement))]
[RequireComponent(typeof(RangedEnemyAttack))]
[RequireComponent(typeof(Rigidbody2D))]
public class ShellshockBoss : Boss
{
    [Header("MOVEMENT SETTINGS")]
    [SerializeField] private float stopDuration = 1.5f;
    [SerializeField] private float moveRange = 5f;

    [SerializeField] private MMFeedbacks stage1Feedback;
    [SerializeField] private MMFeedbacks stage2Feedback;
    [SerializeField] private MMFeedbacks stage3Feedback;

    private EnemyMovement enemyMovement;
    private RangedEnemyAttack rangedAttack;
    private Rigidbody2D rb;
    private Vector2 randomTargetPosition;
    private bool isMoving = true;
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

        if (isMoving)
            enemyMovement.MoveToTargetPosition();
    }


    private void MoveToTarget()
    {
        float distance = Vector2.Distance(transform.position, randomTargetPosition);
        if (distance < 0.2f)
        {
            StopAndAttack();
        }
    }

    private void StopAndAttack()
    {
        isMoving = false;
        isAttacking = true;
        enemyMovement.DisableMovement(stopDuration);

        Invoke(nameof(StartAutoAim), stopDuration * 0.5f);
        Invoke(nameof(ResumeMovement), stopDuration);
    }

    private void StartAutoAim() => rangedAttack.AutoAim();

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

    protected override void ExecuteStageOne()
    {
        stage1Feedback?.PlayFeedbacks();
        StartCoroutine(SpiralShoot());
    }

    private IEnumerator SpiralShoot()
    {
        isAttacking = true;
        enemyMovement.DisableMovement(1.5f);

        float angle = 0f;
        int shots = 6;

        LeanTween.rotateZ(gameObject, transform.eulerAngles.z + 360f, 2f).setEaseLinear();

        for (int i = 0; i < shots; i++)
        {
            LeanTween.scale(gameObject, transform.localScale * 1.05f, 0.1f).setLoopPingPong(1);
            Vector2 dir = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
            rangedAttack.FireCustomDirection(dir.normalized, 3f);
            angle += 60f;
            yield return new WaitForSeconds(0.15f);
        }

        isAttacking = false;
    }

    protected override void ExecuteStageTwo()
    {
        stage2Feedback?.PlayFeedbacks();
        StartCoroutine(ChargeWithBounce());
    }

    private IEnumerator ChargeWithBounce()
    {
        isAttacking = true;
        enemyMovement.DisableMovement(2f);

        LeanTween.scale(gameObject, Vector3.one * 0.7f, 0.2f).setEaseInOutQuad();
        yield return new WaitForSeconds(0.2f);

        Vector2 chargeDirection = (PlayerTransform.position - transform.position).normalized;
        rb.AddForce(chargeDirection * 15f, ForceMode2D.Impulse);

        yield return new WaitForSeconds(1.5f);

        LeanTween.scale(gameObject, Vector3.one, 0.2f).setEaseInOutQuad();
        isAttacking = false;
    }

    protected override void ExecuteStageThree()
    {
        stage3Feedback?.PlayFeedbacks();
        StartCoroutine(ShellBarrage());
    }

    private IEnumerator ShellBarrage()
    {
        isAttacking = true;
        enemyMovement.DisableMovement(2f);

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
