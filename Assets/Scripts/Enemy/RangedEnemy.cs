using System;
using UnityEngine;

[RequireComponent(typeof(EnemyMovement))]
[RequireComponent(typeof(RangedEnemyAttack))]
public class RangedEnemy : Enemy
{

    [Header("Elements")]
    private RangedEnemyAttack attack;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        attack = GetComponent<RangedEnemyAttack>(); 
        attack.StorePlayer(player);
    }

    void Update()
    {
        if (!CanAttack())
            return;

        AttackLogic();

        transform.localScale = player.transform.position.x >
            transform.position.x ? Vector3.one : Vector3.one.With(x: -1);

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
