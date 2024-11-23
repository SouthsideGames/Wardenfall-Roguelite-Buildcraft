using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinionWeapon : MeleeWeapon
{
    [Header("MINION SPECIFICS:")]
    [Tooltip("Reference to the minion prefab that will be spawned.")]
    [SerializeField] private MinionManager minionPrefab;

    [Tooltip("The number of minions to summon each time.")]
    [SerializeField] private int minionCount;

    [Tooltip("The lifetime of each minion in seconds.")]
    [SerializeField] private float minionLifetime;

    [Tooltip("The amount of damage each minion deals.")]
    [SerializeField] private int minionDamage;

    [Tooltip("Cooldown time (in seconds) between consecutive summons.")]
    [SerializeField] private float summonCooldown;

    private float summonTimer;

    

    private void Update()
    {
        summonTimer += Time.deltaTime;
        if (summonTimer >= summonCooldown)
        {
            SummonMinions();
            summonTimer = 0f;
        }
    }

    private void SummonMinions()
    {
        anim.Play("Attack");

        for (int i = 0; i < minionCount; i++)
        {
            // Instantiate a new minion and set its position near the player
            MinionManager minion = Instantiate(minionPrefab, transform.position + Random.insideUnitSphere * 0.5f, Quaternion.identity);

            // Initialize the minion with the specified lifetime and damage
            minion.InitializeMinion(minionLifetime, minionDamage);
        }
    }

    public override void UpdateWeaponStats(CharacterStats _statsManager)
    {
        ConfigureWeaponStats();

        damage = Mathf.RoundToInt(damage * (1 + _statsManager.GetStatValue(Stat.Attack) / 100));
        attackDelay  /= 1 + (_statsManager.GetStatValue(Stat.AttackSpeed) / 100); 

        criticalHitChance = Mathf.RoundToInt(criticalHitChance * 91 + _statsManager.GetStatValue(Stat.CritChance) / 100); 
        criticalHitDamageAmount += _statsManager.GetStatValue(Stat.CritDamage); //Deal times additional damage

    }
}
