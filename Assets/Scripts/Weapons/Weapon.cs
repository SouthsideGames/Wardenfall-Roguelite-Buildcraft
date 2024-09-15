using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public abstract class Weapon : MonoBehaviour
{

    [Header("Settings")]
    [SerializeField] private float range;
    [SerializeField] protected LayerMask enemyMask;


    [Header("Attack")]
    [SerializeField] protected float attackDelay;
    protected float attackTimer;
    [SerializeField] protected int damage;
    
    [Header("Animations")]
    [SerializeField] protected Animator anim;
    [SerializeField] protected float aimLerp;

    protected Enemy closestEnemy;
    protected Vector2 targetUpVector;
    

   protected Enemy GetClosestEnemy()
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

    protected virtual void AutoAim()
    {
        closestEnemy = GetClosestEnemy();

        targetUpVector = Vector3.up;

    }

    private void OnDrawGizmos()
    {

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, range);
    }

}
