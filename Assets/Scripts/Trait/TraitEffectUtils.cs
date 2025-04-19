using System.Collections;
using UnityEngine;

public static class TraitEffectUtils
{
    public static void ApplySpecialEffect(Enemy enemy, TraitTier tier, int stackCount)
    {
        switch (tier.SpecialEffectID)
        {
            case "ArmoredShield":
                CoroutineRunner.Instance.StartCoroutine(ApplyTimedInvincibility(enemy, tier.InvincibilityDuration));
                break;
            case "BerserkerT1":
                enemy.maxHealth = Mathf.FloorToInt(enemy.maxHealth * 0.95f);
                enemy.health = Mathf.Min(enemy.health, enemy.maxHealth);
                break;
            case "BerserkerT2":
                enemy.maxHealth = Mathf.FloorToInt(enemy.maxHealth * 0.90f);
                enemy.health = Mathf.Min(enemy.health, enemy.maxHealth);
                break;
            case "BerserkerT3":
                enemy.maxHealth = Mathf.FloorToInt(enemy.maxHealth * 0.80f);
                enemy.health = Mathf.Min(enemy.health, enemy.maxHealth);
                break;
            case "Gigantism":
                ApplySizeModifier(enemy, tier, stackCount);
                break;
            case "AggressiveT1":
                ApplyAggressiveModifier(enemy, 1.25f, 0.75f);
                break;
            case "AggressiveT2":
                ApplyAggressiveModifier(enemy, 1.5f, 0.5f);
                break;
            case "AggressiveT3":
                ApplyAggressiveModifier(enemy, 2f, 0f); 
                break;
            case "StingyT1":
                enemy.contactDamage = Mathf.FloorToInt(enemy.contactDamage * 0.75f);
                ItemManager.Instance.ModifyDropRates(0.90f); // -10%
                break;
            case "StingyT2":
                enemy.contactDamage = Mathf.FloorToInt(enemy.contactDamage * 0.65f);
                ItemManager.Instance.ModifyDropRates(0.80f); // -20%
                break;
            case "StingyT3":
                enemy.contactDamage = Mathf.FloorToInt(enemy.contactDamage * 0.50f);
                ItemManager.Instance.ModifyDropRates(0.70f); // -30%
                break;
            case "BioChargedT1":
                ApplyBioCharged(enemy, 0.90f, 0.05f);
                break;
            case "BioChargedT2":
                ApplyBioCharged(enemy, 0.85f, 0.075f);
                break;
            case "BioChargedT3":
                ApplyBioCharged(enemy, 0.75f, 0.10f);
                break;
            case "GlassCannonT1":
                ApplyGlassCannon(enemy, 0.7f, 1.5f);
                break;
            case "GlassCannonT2":
                ApplyGlassCannon(enemy, 0.6f, 1.75f);
                break;
            case "GlassCannonT3":
                ApplyGlassCannon(enemy, 0.5f, 2f);
                break;
            case "TimeWarpT1":
                ApplyTimeWarp(enemy, 0.3f, 5f);
                break;
            case "TimeWarpT2":
                ApplyTimeWarp(enemy, 0.5f, 3f);
                break;
            case "TimeWarpT3":
                ApplyTimeWarp(enemy, 0.7f, 2f);
                break;
            case "VampiricT1":
                ApplyVampiric(enemy, 0.1f);
                break;
            case "VampiricT2":
                ApplyVampiric(enemy, 0.2f, true);
                break;
            case "VampiricT3":
                ApplyVampiric(enemy, 0.3f, true);
                break;
            case "HauntingT1":
            case "HauntingT2":
            case "HauntingT3":
                break;
            
        }
    }

    private static IEnumerator ApplyTimedInvincibility(Enemy enemy, float duration)
    {
        while (enemy != null)
        {
            yield return new WaitForSeconds(5f); // Every 5 seconds
            if (enemy != null)
            {
                enemy.isInvincible = true;
                yield return new WaitForSeconds(duration);
                if (enemy != null) enemy.isInvincible = false;
            }
        }
    }

   public static void ApplySizeModifier(Enemy enemy, TraitTier tier, int stack)
    {
        float chance = 0f;
        float scaleFactor = 1.5f; 

        switch (tier.TierName)
        {
            case "Juggernauts": chance = 0.25f; break;
            case "Mutated": chance = 0.5f; break;
            case "Titans": chance = 1f; break;
        }

        if (Random.value <= chance)
        {
            enemy.transform.localScale *= scaleFactor;

            EnemyMovement movement = enemy.GetComponent<EnemyMovement>();
            if (movement != null)
            {
                movement.moveSpeed *= 1f + tier.SpeedModifier; 
            }

        }
    }

    private static void ApplyAggressiveModifier(Enemy enemy, float detectionMultiplier, float critChanceMultiplier)
    {
        enemy.playerDetectionRadius *= detectionMultiplier;

        enemy.modifierHandler.ModifyCritChance(critChanceMultiplier);
    }

    private static void ApplyBioCharged(Enemy enemy, float healthPercent, float damageGainPerTick)
    {
        // Apply reduced health
        enemy.maxHealth = Mathf.FloorToInt(enemy.maxHealth * healthPercent);
        enemy.health = Mathf.Min(enemy.health, enemy.maxHealth);

        // Start damage scaling
        CoroutineRunner.Instance.StartCoroutine(BioChargeOverTime(enemy, damageGainPerTick));
    }

    private static IEnumerator BioChargeOverTime(Enemy enemy, float gainPerTick)
    {
        float interval = 5f;
        EnemyModifierHandler handler = enemy.GetComponent<EnemyModifierHandler>();

        while (enemy != null && handler != null)
        {
            yield return new WaitForSeconds(interval);
            handler.ModifyDamage(gainPerTick);
        }
    }
}




    private static void ApplyGlassCannon(Enemy enemy, float healthMult, float damageMult)
    {
        enemy.maxHealth = Mathf.FloorToInt(enemy.maxHealth * healthMult);
        enemy.health = Mathf.Min(enemy.health, enemy.maxHealth);
        enemy.modifierHandler.ModifyDamage(damageMult - 1f);
    }

    private static void ApplyTimeWarp(Enemy enemy, float speedModifier, float interval)
    {
        CoroutineRunner.Instance.StartCoroutine(TimeWarpRoutine(enemy, speedModifier, interval));
    }

    private static IEnumerator TimeWarpRoutine(Enemy enemy, float speedModifier, float interval)
    {
        EnemyMovement movement = enemy.GetComponent<EnemyMovement>();
        bool fast = true;

        while (enemy != null && movement != null)
        {
            if (fast)
                movement.moveSpeed *= (1f + speedModifier);
            else
                movement.moveSpeed *= (1f - speedModifier);

            fast = !fast;
            yield return new WaitForSeconds(interval);
        }
    }

    private static void ApplyVampiric(Enemy enemy, float lifestealPercent, bool speedupAtLowHealth = false)
    {
        enemy.OnDealDamage += (damage) => {
            int healAmount = Mathf.FloorToInt(damage * lifestealPercent);
            enemy.health = Mathf.Min(enemy.health + healAmount, enemy.maxHealth);
        };

        if (speedupAtLowHealth)
        {
            enemy.OnHealthChanged += () => {
                if (enemy.health < enemy.maxHealth * 0.3f)
                {
                    EnemyMovement movement = enemy.GetComponent<EnemyMovement>();
                    if (movement != null)
                        movement.moveSpeed *= 1.5f;
                }
            };
        }
    }
