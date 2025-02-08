using System;
using UnityEngine;

[RequireComponent(typeof(EnemyMovement))]
public class MeleeEnemy : Enemy
{

    [Header("MELEE SPECIFICS:")]
    [SerializeField] private float attackRate;
    private float attackDelay;

    // Start is called before the first frame update
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

        if(attackTimer >= attackDelay)    
            TryAttack();
        else
            Wait();

        movement.FollowCurrentTarget();
    }

  

    private void Wait()
    {
        attackTimer += Time.deltaTime;
    }

    private void TryAttack()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, character.transform.position);

        if(distanceToPlayer <= playerDetectionRadius)
            Attack();
    }


}
