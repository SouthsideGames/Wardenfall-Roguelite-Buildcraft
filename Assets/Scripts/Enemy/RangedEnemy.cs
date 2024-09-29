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

    void Update()
    {
        if (!CanAttack())
            return;

        AttackLogic();

        transform.localScale = character.transform.position.x >
            transform.position.x ? Vector3.one : Vector3.one.With(x: -1);

    }

    private void AttackLogic()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, character.transform.position);

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
