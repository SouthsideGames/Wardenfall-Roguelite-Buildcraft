using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    enum State 
    {
        Idle,
        Attack
    }
    private State state;

    [Header("Elements")]
    [SerializeField] private Transform hitpoint;
    [SerializeField] private float hitDetectionRadius;
    private List<Enemy> damagedEnemies = new List<Enemy>();

    [Header("Attack")]
    [SerializeField] private float attackDelay;
    private float attackTimer;
    [SerializeField] private int damage;
    
    [Header("Settings")]
    [SerializeField] private float range;
    [SerializeField] private LayerMask enemyMask;

    [Header("Animations")]
    [SerializeField] private Animator anim;
    [SerializeField] private float aimLerp;
    
    [Header("Debug")]
    [SerializeField] private bool showGizmos;

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

    private void AutoAim()
    {
        Enemy closestEnemy = GetClosestEnemy();

        Vector2 targetUpVector = Vector3.up;

        if(closestEnemy != null)
        {
            ManageAttackTimer();
            targetUpVector = (closestEnemy.transform.position - transform.position).normalized;
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

    private Enemy GetClosestEnemy()
    {
        Enemy closestEnemy = null;
        Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, range, enemyMask);

        if(enemies.Length <= 0)
            return null;

        float minDistance = range;

        for (int i = 0; i < enemies.Length; i++)
        {
            Enemy enemyChecked = enemies[i].GetComponent<Enemy>();    

            float distanceToEnemy = Vector2.Distance(transform.position, enemyChecked.transform.position);  

            if(distanceToEnemy < minDistance)
            {
                closestEnemy = enemyChecked;
                minDistance = distanceToEnemy;  
            }
        }


        return closestEnemy;
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
    }

    private void EndAttack()
    {
        state = State.Idle;
        damagedEnemies.Clear(); 
    }

    private void AttackLogic()
    {
        Collider2D[] enemies = Physics2D.OverlapCircleAll(hitpoint.position, hitDetectionRadius, enemyMask);

        for (int i = 0; i < enemies.Length; i++)
        {
            Enemy enemy = enemies[i].GetComponent<Enemy>();

            if(!damagedEnemies.Contains(enemy))
            {
                enemy.TakeDamage(damage);
                damagedEnemies.Add(enemy);
            }
           
        }
    }

    private void OnDrawGizmos()
    {
        if(!showGizmos)
            return;

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, range);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(hitpoint.position, hitDetectionRadius);
    }
}
