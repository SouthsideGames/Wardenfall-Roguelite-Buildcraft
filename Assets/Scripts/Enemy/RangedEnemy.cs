using System;
using UnityEngine;

//Range Enemies can not do critical damage
[RequireComponent(typeof(EnemyMovement))]
[RequireComponent(typeof(RangedEnemyAttack))]
public class RangedEnemy : Enemy
{

    [Header("RANGED SPECIFICS:")]
    private RangedEnemyAttack attack;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        attack = GetComponent<RangedEnemyAttack>(); 
        attack.StorePlayer(character);
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

        if(distanceToPlayer <= playerDetectionRadius)
            TryAttack();
        else
            movement.FollowPlayer();
    }


    
    protected virtual void TryAttack()
    {
       attack.AutoAim();
    }


}
