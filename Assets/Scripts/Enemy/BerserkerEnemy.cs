using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BerserkerEnemy : Enemy
{
    [Header("BERSERKER SPECIFICS:")]
    [SerializeField] private float enragedThreshold = 0.5f;
    [SerializeField] private float enragedAttackMultiplier = 2f;
    [SerializeField] private float enragedSpeedMultiplier = 1.5f;
    
    [SerializeField] private float attackRate;
    private float attackDelay;
    private bool hasEnraged = false; 

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

        if (attackTimer >= attackDelay)    
            TryAttack();
        else
            Wait();

        if (!hasEnraged && health <= maxHealth * enragedThreshold)
        {
            Enrage();
        }

        if (!hasEnraged)
        {
            anim.Play("Move");
        }
        else
        {
            anim.Play("Berserk");
        }

        movement.FollowCurrentTarget();
    }

    private void Enrage()
    {
        damage = Mathf.RoundToInt(damage * enragedAttackMultiplier);
        movement.moveSpeed *= enragedSpeedMultiplier;

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
