using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//TODO: Add visual effect for the tethered enemies
public class HealerEnemy : Enemy
{
    [Header("HEALER SPECIFICS:")]
    [SerializeField] private float tetherRadius = 5f; // Radius within which the totem will search for enemies to tether
    [SerializeField] private int maxTetheredEnemies = 4; // Maximum number of enemies that can be tethered
    [SerializeField] private LayerMask enemyLayer; // Layer mask to ensure only enemies are detected

    private List<Enemy> tetheredEnemies = new List<Enemy>(); // List of tethered enemies

    protected override void SpawnCompleted()
    {
        base.SpawnCompleted();

        TetherRandomEnemies();
    }
    

    private void TetherRandomEnemies()
    {
        // Find all enemies within the tether radius that are not invincible
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, tetherRadius, enemyLayer);
        List<Enemy> potentialEnemies = new List<Enemy>();

        foreach (Collider2D hit in hits)
        {
            // Check if the hit object is an enemy and is not already invincible
            if (hit.TryGetComponent<Enemy>(out Enemy enemy) && enemy != this && !enemy.isInvincible)
            {
                potentialEnemies.Add(enemy);
            }
        }

        if (potentialEnemies.Count == 0)
        {
            Debug.Log("No eligible enemies found within tether radius.");
            return;
        }

        // Shuffle the list of potential enemies and select up to maxTetheredEnemies
        while (tetheredEnemies.Count < maxTetheredEnemies && potentialEnemies.Count > 0)
        {
            int randomIndex = Random.Range(0, potentialEnemies.Count);
            Enemy selectedEnemy = potentialEnemies[randomIndex];
            tetheredEnemies.Add(selectedEnemy);
            selectedEnemy.isInvincible = true; // Make the selected enemy invincible
            potentialEnemies.RemoveAt(randomIndex);
        }

        Debug.Log($"Totem tethered {tetheredEnemies.Count} enemies, making them invincible.");
    }



    public override void Die()
    {
        base.Die(); 
        UntetherEnemies();
    }


    private void UntetherEnemies()
    {
        foreach (Enemy enemy in tetheredEnemies)
        {
            if (enemy != null)
            {
                enemy.isInvincible = false; // Remove invincibility from tethered enemies
            }
        }

        tetheredEnemies.Clear();

    }

    private void OnDrawGizmosSelected()
    {
        // Draw the tether radius in the editor for visualization
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, tetherRadius);
    }
}
