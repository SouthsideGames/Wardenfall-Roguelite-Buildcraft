using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinionMovement : MonoBehaviour
{
   [Header("SETTINGS:")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float detectionRange;
    [SerializeField] private LayerMask enemyMask;

    private Rigidbody2D rb;
    private Enemy targetEnemy;
    private MinionAttack minionAttack;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        minionAttack = GetComponent<MinionAttack>();
    }

    private void Update()
    {
        // Find the closest enemy if one is not already targeted
        if (targetEnemy == null || !targetEnemy.isActiveAndEnabled)
        {
            targetEnemy = FindClosestEnemy();
            minionAttack.SetTargetEnemy(targetEnemy); // Set target for MinionAttack
        }

        // Move towards the target if there is one
        if (targetEnemy != null)
        {
            MoveTowardsEnemy();
        }
    }

    private Enemy FindClosestEnemy()
    {
        Collider2D[] enemiesInRange = Physics2D.OverlapCircleAll(transform.position, detectionRange, enemyMask);
        Enemy closestEnemy = null;
        float closestDistance = Mathf.Infinity;

        foreach (Collider2D enemyCollider in enemiesInRange)
        {
            Enemy enemy = enemyCollider.GetComponent<Enemy>();
            float distance = Vector2.Distance(transform.position, enemy.transform.position);
            if (distance < closestDistance)
            {
                closestEnemy = enemy;
                closestDistance = distance;
            }
        }

        return closestEnemy;
    }

    private void MoveTowardsEnemy()
    {
        Vector2 direction = (targetEnemy.transform.position - transform.position).normalized;
        rb.velocity = direction * moveSpeed;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}
