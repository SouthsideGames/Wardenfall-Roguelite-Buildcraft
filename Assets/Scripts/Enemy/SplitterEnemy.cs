using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplitterEnemy : Enemy
{
    [Header("SPLITTER SPECIFIC:")]
    [SerializeField] private int maxSplits = 3; // Maximum number of times the enemy can split
    [SerializeField] private GameObject smallerVersionPrefab; // Prefab for the smaller version of the enemy
    [SerializeField] private float splitScaleFactor = 0.5f; // Scale reduction factor for each split
    [SerializeField] private int splitHealthFactor = 2; // Health reduction factor for each split

    [Header("ATTACK:")]
    [SerializeField] private int damage;
    [SerializeField] private float attackRate;
    private float attackDelay;
    private float attackTimer;

    private int splitCount = 0; // Number of times the enemy has split


    protected override void Die()
    {
        if (splitCount < maxSplits)
        {
            Split();
        }
        else
        {
            base.Die(); // Call the base Die method to handle death
        }
    }

    private void Split()
    {
        // Increase the split count
        splitCount++;

        // Create two smaller enemies
        for (int i = 0; i < 2; i++)
        {
            // Instantiate a smaller version of the enemy
            GameObject smallerEnemy = Instantiate(smallerVersionPrefab, transform.position, Quaternion.identity);

            // Adjust the size and health of the smaller enemy
            SplitterEnemy smallerEnemyScript = smallerEnemy.GetComponent<SplitterEnemy>();

            if (smallerEnemyScript != null)
            {
                // Scale down the enemy
                smallerEnemy.transform.localScale = transform.localScale * splitScaleFactor;

                // Reduce the health
                smallerEnemyScript.maxHealth = maxHealth / splitHealthFactor;
                smallerEnemyScript.health = smallerEnemyScript.maxHealth;

                // Inherit the split count
                smallerEnemyScript.splitCount = splitCount;
            }
        }

        // Destroy the current enemy after splitting
        Destroy(gameObject);
    }

    private void Update()
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
        float distanceToPlayer = Vector2.Distance(transform.position, character.transform.position);

        if(distanceToPlayer <= playerDetectionRadius)
            Attack();
    }

    private void Attack()
    {

        attackTimer = 0;
        character.TakeDamage(damage);
    }

   
}
