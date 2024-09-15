using System;
using UnityEngine;

[RequireComponent(typeof(EnemyMovement))]
public class MeleeEnemy : Enemy
{

    [Header("Attack")]
    [SerializeField] private int damage;
    [SerializeField] private float attackRate;
    private float attackDelay;
    private float attackTimer;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        attackDelay = 1f / attackRate;

    }

    void Update()
    {
        if (!CanAttack())
            return;

        if(attackTimer >= attackDelay)    
            TryAttack();
        else
            Wait();

        movement.FollowPlayer();
    }

  

    private void Wait()
    {
        attackTimer += Time.deltaTime;
    }

    private void TryAttack()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);

        if(distanceToPlayer <= playerDetectionRadius)
            Attack();
    }

    private void Attack()
    {

        attackTimer = 0;
        player.TakeDamage(damage);
    }


}
