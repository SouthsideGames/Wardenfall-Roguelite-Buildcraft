using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TwinfangBoss : Boss
{
    [Header("TWINFANG")]
    [SerializeField] private float attackRange = 3f;

    private EnemyMovement enemyMovement;
    private bool isAttacking;
    private Vector3 originalScale;

    protected override void InitializeBoss()
    {
        base.InitializeBoss();
        enemyMovement = GetComponent<EnemyMovement>();
        originalScale = transform.localScale;
    }

    protected override void ExecuteStageOne()
    {
        LungeAttack();
    }

    private void LungeAttack()
    {
        if (isAttacking) return;
        isAttacking = true;
        enemyMovement.DisableMovement(0.6f);

        Vector2 attackDirection = (playerTransform.position - transform.position).normalized;

        // Stretch before lunging
        transform.localScale = new Vector3(originalScale.x * 1.5f, originalScale.y * 0.7f, originalScale.z);

        // Quick lunge forward
        transform.position += (Vector3)attackDirection * attackRange;

        // Squash effect after landing
        transform.localScale = new Vector3(originalScale.x * 0.7f, originalScale.y * 1.3f, originalScale.z);

        // Jump backward to reset distance
        transform.position -= (Vector3)attackDirection * (attackRange * 0.75f);

        transform.localScale = originalScale;
        isAttacking = false;
    }
}