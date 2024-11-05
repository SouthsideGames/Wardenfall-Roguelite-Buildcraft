using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinionWeapon : Weapon
{
    [Header("MINION SPECIFICS:")]
    [SerializeField] private MinionManager minionPrefab;  // Reference to the minion prefab
    [SerializeField] private int minionCount;             // Number of minions to summon
    [SerializeField] private float minionLifetime;        // Lifetime of each minion
    [SerializeField] private int minionDamage;            // Damage dealt by each minion
    [SerializeField] private float summonCooldown;        // Time between summons

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
