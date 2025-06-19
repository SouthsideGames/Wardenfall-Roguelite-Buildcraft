using UnityEngine;

public class SplitterEnemy : Enemy
{
    [Header("SPLITTER SPECIFIC:")]
    [SerializeField, Tooltip("Maximum number of times the enemy can split")] private int maxSplits = 3; 
    [SerializeField, Tooltip("Prefab for the smaller version of the enemy")] private GameObject smallerVersionPrefab; 
    [SerializeField, Tooltip("Scale reduction factor for each split")] private float splitScaleFactor = 0.5f; 
    [SerializeField, Tooltip("Health reduction factor for each split")] private int splitHealthFactor = 2; 

    private float attackDelay;
    private int splitCount = 0; 


    public override void Die()
    {
        if (splitCount < maxSplits)
            Split();
        else
            base.Die(); 
    }

    private void Split()
    {
        splitCount++;

        for (int i = 0; i < 2; i++)
        {
            GameObject smallerEnemy = Instantiate(smallerVersionPrefab, transform.position, Quaternion.identity);

            SplitterEnemy smallerEnemyScript = smallerEnemy.GetComponent<SplitterEnemy>();

            if (smallerEnemyScript != null)
            {
                smallerEnemy.transform.localScale = transform.localScale * splitScaleFactor;

                smallerEnemyScript.maxHealth = maxHealth / splitHealthFactor;
                smallerEnemyScript.health = smallerEnemyScript.maxHealth;

                smallerEnemyScript.splitCount = splitCount;
            }
        }

        Destroy(gameObject);
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
