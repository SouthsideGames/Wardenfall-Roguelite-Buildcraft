using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealerEnemy : Enemy
{
    [Header("Totem Specific")]
    [SerializeField] private float tetherRadius = 5f; // Radius within which the totem will search for enemies to tether
    [SerializeField] private int maxTetheredEnemies = 4; // Maximum number of enemies that can be tethered
    [SerializeField] private LineRenderer tetherEffect; // Optional visual effect for the tether

    private List<Enemy> tetheredEnemies = new List<Enemy>(); // List of tethered enemies

    protected override void Start()
    {
        base.Start();
        TetherRandomEnemies();
    }

    private void TetherRandomEnemies()
    {
        // Find all enemies within the tether radius that are not invincible
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, tetherRadius);
        List<Enemy> potentialEnemies = new List<Enemy>();

        foreach (Collider2D hit in hits)
        {
            // Check if the hit object is an enemy and is not already invincible
            if (hit.TryGetComponent<Enemy>(out Enemy enemy) && enemy != this && !enemy.isInvincible)
            {
                potentialEnemies.Add(enemy);
            }
        }

        // Shuffle the list of potential enemies and select up to maxTetheredEnemies
        while (tetheredEnemies.Count < maxTetheredEnemies && potentialEnemies.Count > 0)
        {
            int randomIndex = Random.Range(0, potentialEnemies.Count);
            Enemy selectedEnemy = potentialEnemies[randomIndex];
            tetheredEnemies.Add(selectedEnemy);
            selectedEnemy.isInvincible = true; // Make the selected enemy invincible
            CreateTetherEffect(selectedEnemy);
            potentialEnemies.RemoveAt(randomIndex);
        }

        Debug.Log($"Totem tethered {tetheredEnemies.Count} enemies, making them invincible.");
    }

    private void CreateTetherEffect(Enemy enemy)
    {
        // Optionally create a visual tether effect using a LineRenderer or similar component
        if (tetherEffect != null)
        {
            LineRenderer effect = Instantiate(tetherEffect, transform.position, Quaternion.identity);
            effect.SetPosition(0, transform.position);
            effect.SetPosition(1, enemy.transform.position);
            effect.transform.SetParent(enemy.transform);
        }
    }

    protected override void Die()
    {
        // Check if the Healer dies and handle untethering
        if (health <= 0)
        {
            UntetherEnemies();
        }

        deathParticles.transform.SetParent(null);
        deathParticles.Play();

        Destroy(gameObject);
    }


    private void UntetherEnemies()
    {
        foreach (Enemy enemy in tetheredEnemies)
        {
            if (enemy != null)
            {
                enemy.isInvincible= false; // Remove invincibility from tethered enemies
            }
        }

        tetheredEnemies.Clear();
        Debug.Log("Totem has died. Tethered enemies are no longer invincible.");
    }

    private void OnDrawGizmosSelected()
    {
        // Draw the tether radius in the editor for visualization
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, tetherRadius);
    }
}
