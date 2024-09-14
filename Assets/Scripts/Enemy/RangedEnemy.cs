using System;
using UnityEngine;

[RequireComponent(typeof(EnemyMovement))]
[RequireComponent(typeof(RangedEnemyAttack))]
public class RangedEnemy : Enemy
{

    [Header("Elements")]
    private RangedEnemyAttack attack;

    // Start is called before the first frame update
    void Start()
    {
        attack = GetComponent<RangedEnemyAttack>(); 
        attack.StorePlayer(player);
    }

    void Update()
    {
     
        AttackLogic();

    }

    private void AttackLogic()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);

        if(distanceToPlayer <= playerDetectionRadius)
            TryAttack();
        else
            movement.FollowPlayer();
    }


    private void TryAttack()
    {
       attack.AutoAim();
    }


}
