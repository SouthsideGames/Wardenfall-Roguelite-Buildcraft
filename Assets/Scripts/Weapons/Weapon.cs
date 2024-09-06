using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float range;
    [SerializeField] private LayerMask enemyMask;

    [Header("Animations")]
    [SerializeField] private float aimLerp;
    
    [Header("Debug")]
    [SerializeField] private bool showGizmos;

    // Update is called once per frame
    void Update()
    {
       AutoAim();
    }

    private void AutoAim()
    {
        Enemy closestEnemy = GetClosestEnemy();

        Vector2 targetUpVector = Vector3.up;

        if(closestEnemy != null)
            targetUpVector = (closestEnemy.transform.position - transform.position).normalized;

        transform.up = Vector3.Lerp(transform.up, targetUpVector, Time.deltaTime * aimLerp);
    }

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

    private void OnDrawGizmos()
    {
        if(!showGizmos)
            return;

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}
