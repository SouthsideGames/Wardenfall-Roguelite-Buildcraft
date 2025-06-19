using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EnemyMovement))]
public class HealerEnemy : Enemy
{
    [Header("HEALING SETTINGS")]
    [SerializeField] private float healRadius = 5.5f;
    [SerializeField] private int healAmount = 10;
    [SerializeField] private float healCooldown = 2.5f;

    [Header("VFX/SFX")]
    [SerializeField] private GameObject healVFX;
    [SerializeField] private AudioClip healSFX;

    private float healTimer = 0f;

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();
        healTimer -= Time.deltaTime;

        if (healTimer <= 0f)
        {
            TryHeal();
            healTimer = healCooldown;
        }
    }

    private void TryHeal()
    {
        Enemy target = FindClosestWoundedAlly(healRadius);
        if (target != null)
        {
            target.Heal(healAmount);

            if (healVFX != null)
                Instantiate(healVFX, target.transform.position, Quaternion.identity);

            if (healSFX != null)
                AudioSource.PlayClipAtPoint(healSFX, transform.position);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, healRadius);
    }
}
