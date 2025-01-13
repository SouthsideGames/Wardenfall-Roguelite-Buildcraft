using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BerserkerEnemy : Enemy
{
    [Header("BERSERKER SPECIFICS:")]
    [SerializeField] private float enragedThreshold = 0.5f;  // When health is below 50%, it becomes enraged
    [SerializeField] private float enragedAttackMultiplier = 2f;
    [SerializeField] private float enragedSpeedMultiplier = 1.5f;
    
    [SerializeField] private float attackRate;
    private float attackDelay;
    private bool hasEnraged = false;  // Flag to check if the enemy has already enraged

    protected override void Start()
    {
        base.Start();
        attackDelay = 1f / attackRate;
    }

    protected override void Update()
    {
        base.Update();
        
        if (!CanAttack())
            return;

        // Handle attacking logic
        if (attackTimer >= attackDelay)    
            TryAttack();
        else
            Wait();

        // Check if health is below threshold and enrage if not already enraged
        if (!hasEnraged && health <= maxHealth * enragedThreshold)
        {
            Enrage();
        }

        // Continue following the player
        movement.FollowCurrentTarget();
    }

    private void Enrage()
    {
        // Only enrage once
        damage = Mathf.RoundToInt(damage * enragedAttackMultiplier);
        movement.moveSpeed *= enragedSpeedMultiplier;

        // Set the flag to prevent further enraging
        hasEnraged = true;
    }

    private void Wait()
    {
        attackTimer += Time.deltaTime;
    }

    private void TryAttack()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, character.transform.position);

        if (distanceToPlayer <= playerDetectionRadius)
        {
            Attack();
        }
    }
}
