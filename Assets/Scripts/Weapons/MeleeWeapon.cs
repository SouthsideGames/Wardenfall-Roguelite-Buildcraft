using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeWeapon : Weapon
{
    enum State 
    {
        Idle,
        Attack
    }

    private State state;

    [Header("ELEMENTS:")]
    [SerializeField] private Transform hitpoint;
    [SerializeField] private BoxCollider2D hitCollider;
    private List<Enemy> damagedEnemies = new List<Enemy>();

    // Start is called before the first frame update
    void Start()
    {
         state = State.Idle;
    }

    // Update is called once per frame
    void Update()
    {
        switch(state)
       {
            case State.Idle:
                AutoAim();
                break;
            case State.Attack:
                AttackState();
                break;
            default:
                break;
       }
    }

    private void AttackState()
    {
        AttackLogic();
    }

    private void StartAttack()
    {
        anim.Play("Attack");
        state = State.Attack;   

        damagedEnemies.Clear(); 

        anim.speed = 1f / attackDelay;
    }

    private void EndAttack()
    {
        state = State.Idle;
        damagedEnemies.Clear(); 
    }

    private void AttackLogic()
    {
        Collider2D[] enemies = Physics2D.OverlapBoxAll
        (
            hitpoint.position, 
            hitCollider.bounds.size, 
            hitpoint.localEulerAngles.z, 
            enemyMask
        );

        for (int i = 0; i < enemies.Length; i++)
        {
            Enemy enemy = enemies[i].GetComponent<Enemy>();

            if(!damagedEnemies.Contains(enemy))
            {
                int damage = GetDamage(out bool isCriticalHit);

                enemy.TakeDamage(damage, isCriticalHit);
                damagedEnemies.Add(enemy);
            }
           
        }
    }

    protected override void AutoAim()
    {
        base.AutoAim();

        if(closestEnemy != null)
        {
            targetUpVector = (closestEnemy.transform.position - transform.position).normalized;
            transform.up = targetUpVector;  
            ManageAttackTimer();
        }

        transform.up = Vector3.Lerp(transform.up, targetUpVector, Time.deltaTime * aimLerp);

        Wait();

    }

    private void ManageAttackTimer()
    {
        if(attackTimer >= attackDelay)
        {
            attackTimer = 0;
            StartAttack();
        }
    }

    private void Wait() => attackTimer += Time.deltaTime;


}
