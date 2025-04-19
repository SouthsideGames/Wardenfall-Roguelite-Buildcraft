using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TwinfangBoss : Boss
{
    [Header("STAGE 1")]
    [SerializeField] private float attackRange = 3f;

    private EnemyMovement enemyMovement;
    private bool isAttacking;
    private Vector3 originalScale;

    protected override void InitializeBoss()
    {
        base.InitializeBoss();
        enemyMovement = GetComponent<EnemyMovement>();
        originalScale = transform.localScale;
        attackTimer = attackCooldown;
    }

    protected override void Update()
    {
        base.Update();

        if (!hasSpawned || isAttacking) return;

        attackTimer -= Time.deltaTime;

        if (Vector2.Distance(transform.position, playerTransform.position) <= attackRange && attackTimer <= 0)
        {
            ExecuteStage();
            attackTimer = attackCooldown;
        }
        else
            enemyMovement.FollowCurrentTarget();
    }

    protected override void ExecuteStageOne() => CoroutineRunner.Instance.RunPooled(LungeAttack());

    private IEnumerator LungeAttack()
    {
        if (isAttacking) yield break;

        isAttacking = true;
        enemyMovement.DisableMovement(0.6f);
        Vector2 attackDirection = (playerTransform.position - transform.position).normalized;

        transform.localScale = new Vector3(originalScale.x * 1.5f, originalScale.y * 0.7f, originalScale.z);
        yield return new WaitForSeconds(0.2f);

        Vector2 startPos = transform.position;
        Vector2 lungePos = startPos + attackDirection * attackRange;

        float elapsedTime = 0f;
        while (elapsedTime < 0.2f)
        {
            transform.position = Vector2.Lerp(startPos, lungePos, elapsedTime / 0.2f);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = lungePos;

        transform.localScale = new Vector3(originalScale.x * 0.7f, originalScale.y * 1.3f, originalScale.z);
        yield return new WaitForSeconds(0.1f);

        Vector2 retreatPos = lungePos - attackDirection * (attackRange * 0.75f);

        elapsedTime = 0f;
        while (elapsedTime < 0.2f)
        {
            transform.position = Vector2.Lerp(lungePos, retreatPos, elapsedTime / 0.2f);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = retreatPos;

        transform.localScale = originalScale;
        isAttacking = false;
    }
}