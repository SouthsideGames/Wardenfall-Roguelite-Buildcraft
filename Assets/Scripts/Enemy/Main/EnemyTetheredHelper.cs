using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Enemy))]
public class EnemyTetheredHelper : MonoBehaviour
{
    private Enemy enemy;
    private float lastHealth;
    private float damageSharePercent;
    private List<EnemyTetheredHelper> tetherGroup;

    private void Awake()
    {
        enemy = GetComponent<Enemy>();
        enabled = false; // Start disabled by default
    }

    public void Initialize(float sharePercent)
    {
        damageSharePercent = sharePercent;
        lastHealth = enemy.CurrentHealth;
    }

    public void SetTetherGroup(List<EnemyTetheredHelper> group)
    {
        tetherGroup = group;
    }

    private void OnEnable()
    {
        if (enemy != null)
        {
            lastHealth = enemy.CurrentHealth;
        }
    }

    private void Update()
    {
        if (enemy == null || tetherGroup == null || tetherGroup.Count <= 1)
            return;

        float currentHealth = enemy.CurrentHealth;
        float damageTaken = lastHealth - currentHealth;

        if (damageTaken > 0f)
        {
            foreach (var tether in tetherGroup)
            {
                if (tether != null && tether != this && tether.enabled)
                {
                    tether.ReceiveSharedDamage(damageTaken * damageSharePercent);
                }
            }
        }

        lastHealth = currentHealth;
    }

    public void ReceiveSharedDamage(float amount)
    {
        enemy.TakeDamage((int)amount);
    }
}
