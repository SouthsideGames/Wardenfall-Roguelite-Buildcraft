using System;
using UnityEngine;

// Range Enemies can not do critical damage
[RequireComponent(typeof(EnemyMovement))]
[RequireComponent(typeof(RangedEnemyAttack))]
public class RangedEnemy : Enemy
{
    private RangedEnemyAttack attack;
    protected EnemyAnimator enemyAnimator;

    protected override void Start()
    {
        base.Start();

        attack = GetComponent<RangedEnemyAttack>(); 
        attack.StorePlayer(character);

        enemyAnimator = GetComponent<EnemyAnimator>();
        enemyAnimator?.PlayIdlePulseAnimation();
    }

    protected override void Update()
    {
        base.Update();  

        if (!CanAttack())
            return;

        AttackLogic();
    }

    private void AttackLogic()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, character.transform.position);

        if (distanceToPlayer <= playerDetectionRadius)
            TryAttack();
    }

    protected virtual void TryAttack()
    {
        attack.AutoAim();
        enemyAnimator?.PlayAttackBurst();
    }
}
