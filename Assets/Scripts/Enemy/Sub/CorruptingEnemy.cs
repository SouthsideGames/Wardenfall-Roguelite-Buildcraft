using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CorruptingEnemy : Enemy
{
    [Header("Corruption Settings")]
    [SerializeField] private float corruptionRadius = 6f;
    [SerializeField] private float corruptionDuration = 8f;
    [SerializeField] private float corruptInterval = 3f;
    [SerializeField] private float damageMultiplier = 1.5f;
    [SerializeField] private float speedMultiplier = 1.3f;
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private ParticleSystem corruptionEffect;
    
    private float corruptTimer;
    private readonly List<Enemy> corruptedEnemies = new();

    protected override void Start()
    {
        base.Start();
        corruptTimer = corruptInterval;
    }

    protected override void Update()
    {
        base.Update();
        
        if (!hasSpawned) return;

        corruptTimer -= Time.deltaTime;
        if (corruptTimer <= 0)
        {
            CorruptNearbyEnemies();
            corruptTimer = corruptInterval;
        }
    }

    private void CorruptNearbyEnemies()
    {
        Collider2D[] nearbyEnemies = Physics2D.OverlapCircleAll(transform.position, corruptionRadius, enemyLayer);
        
        foreach (Collider2D col in nearbyEnemies)
        {
            Enemy enemy = col.GetComponent<Enemy>();
            if (enemy != null && enemy != this && !corruptedEnemies.Contains(enemy))
            {
                StartCoroutine(CorruptEnemy(enemy));
            }
        }
    }

    private IEnumerator CorruptEnemy(Enemy enemy)
    {
        if (enemy == null) yield break;

        // Add to corrupted list
        corruptedEnemies.Add(enemy);
        
        EnemyModifierHandler modHandler = enemy.modifierHandler;
        if (modHandler != null)
        {
            modHandler.ModifyDamage(damageMultiplier - 1f);
            modHandler.ModifySpeed(speedMultiplier - 1f);
            
            if (corruptionEffect != null)
            {
                ParticleSystem effect = Instantiate(corruptionEffect, enemy.transform);
                effect.Play();
            }
        }

        // Wait for duration
        yield return new WaitForSeconds(corruptionDuration);

        // Remove corruption if enemy still exists
        if (enemy != null && modHandler != null)
        {
            modHandler.ModifyDamage(-(damageMultiplier - 1f));
            modHandler.ModifySpeed(-(speedMultiplier - 1f));
            corruptedEnemies.Remove(enemy);
        }
    }

    public override void Die()
    {
        // Remove all corruptions when killed
        foreach (Enemy enemy in corruptedEnemies.ToArray())
        {
            if (enemy != null && enemy.modifierHandler != null)
            {
                enemy.modifierHandler.ModifyDamage(-(damageMultiplier - 1f));
                enemy.modifierHandler.ModifySpeed(-(speedMultiplier - 1f));
            }
        }
        corruptedEnemies.Clear();
        
        base.Die();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, corruptionRadius);
    }
}