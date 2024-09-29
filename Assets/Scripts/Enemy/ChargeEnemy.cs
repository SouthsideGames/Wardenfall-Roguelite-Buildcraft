using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargeEnemy : Enemy
{
    [Header("Charger Specific")]
    [SerializeField] private float chargeTime = 2f; // Time spent charging up before dashing
    [SerializeField] private float growFactor = 1.5f; // How much the enemy grows during the charge
    [SerializeField] private float chargeSpeed = 20f; // Speed of the dash towards the player
    [SerializeField] private float chargeDistance = 10f; // Maximum distance the enemy will charge
    [SerializeField] private float cooldownTime = 3f; // Time spent on cooldown after charging

    [Header("Attack")]
    [SerializeField] private int damage;
    [SerializeField] private float attackRate;
    private bool attackPerformed = false; // Flag to check if the attack has been performed during the charge

    private Vector3 originalScale; // To keep track of the enemy's original scale
    private Vector2 chargeDirection; // Direction to charge towards the player
    private bool isCharging = false; // Indicates whether the enemy is currently in a charging sequence

    protected override void Start()
    {
        base.Start();
        originalScale = transform.localScale;
    }

    void Update()
    {
        // Check if the enemy is currently not charging, then start the charging routine
        if (!isCharging)
        {
            StartCoroutine(ChargeRoutine());
        }
    }

    private IEnumerator ChargeRoutine()
    {
        isCharging = true; // Set to true to indicate the enemy is in the charging sequence
        attackPerformed = false; // Reset the attack flag at the start of the charge

        // 1. Stay idle for a brief moment
        yield return new WaitForSeconds(1f);

        // 2. Grow in size over chargeTime
        yield return StartCoroutine(Grow());

        // 3. Locate the player and set the charge direction
        LocatePlayer();

        // 4. Charge towards the player
        yield return StartCoroutine(DashTowardsPlayer());

        // 5. Shrink back to original size
        yield return StartCoroutine(Shrink());

        // 6. Cooldown before repeating
        yield return new WaitForSeconds(cooldownTime);

        isCharging = false; // Reset to false to allow the coroutine to repeat
    }

    private IEnumerator Grow()
    {
        float elapsed = 0f;
        Vector3 targetScale = originalScale * growFactor;

        while (elapsed < chargeTime)
        {
            transform.localScale = Vector3.Lerp(originalScale, targetScale, elapsed / chargeTime);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localScale = targetScale; // Ensure it reaches the full size
    }

    private void LocatePlayer()
    {
        if (character != null)
        {
            chargeDirection = (character.transform.position - transform.position).normalized; // Calculate the direction towards the player
        }
        else
        {
            chargeDirection = Vector2.right; // Default direction if player is not found
        }
    }

    private IEnumerator DashTowardsPlayer()
    {
        // Calculate the target position based on the charge distance and direction
        Vector2 startPosition = transform.position;
        Vector2 targetPosition = startPosition + chargeDirection * chargeDistance;

        float distanceTraveled = 0f;

        // Dash towards the player until the charge distance is covered
        while (distanceTraveled < chargeDistance)
        {
            float step = chargeSpeed * Time.deltaTime; // Calculate the step for this frame
            transform.position = Vector2.MoveTowards(transform.position, targetPosition, step);
            distanceTraveled += step;

            TryAttack(); // Attempt to attack during the charge

            yield return null;
        }
    }

    private IEnumerator Shrink()
    {
        float elapsed = 0f;
        Vector3 targetScale = originalScale;

        while (elapsed < chargeTime)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, targetScale, elapsed / chargeTime);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localScale = targetScale; // Ensure it returns to the original size
    }

    private void TryAttack()
    {
        // Check if the enemy is close enough to the player to attack and hasn't attacked yet
        float distanceToPlayer = Vector2.Distance(transform.position, character.transform.position);

        if (distanceToPlayer <= playerDetectionRadius && !attackPerformed)
        {
            Attack();
        }
    }

    private void Attack()
    {
        attackPerformed = true; // Set the flag to true to prevent further attacks during this charge
        character.TakeDamage(damage); // Inflict damage on the player
        Debug.Log("ChargeEnemy attacked the player!");
    }

    private void OnDrawGizmosSelected()
    {
        // Visualize the charge direction and potential distance in the editor
        if (character != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, transform.position + (Vector3)chargeDirection * chargeDistance);
        }
    }
}
