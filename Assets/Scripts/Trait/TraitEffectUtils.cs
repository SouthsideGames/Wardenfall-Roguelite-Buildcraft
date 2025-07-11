using System.Collections;
using UnityEngine;

public static class TraitEffectUtils
{
    public static void ApplySpecialEffect(Enemy enemy, TraitTier tier, int stackCount)
    {
        switch (tier.SpecialEffectID)
        {
            case "ArmoredShield":
                ApplyTimedInvincibility(enemy, 3f); // or whatever duration you decide
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
            case "HauntingT1":
            case "HauntingT2":
            case "HauntingT3":
                break;
            case "LowYieldT1":
                enemy.maxHealth = Mathf.FloorToInt(enemy.maxHealth * 0.90f);
                CharacterManager.Instance.cards.ModifyCardCap(-1);
                break;
            case "LowYieldT2":
                enemy.maxHealth = Mathf.FloorToInt(enemy.maxHealth * 0.80f);
                CharacterManager.Instance.cards.ModifyCardCap(-2);
                break;
            case "LowYieldT3":
                enemy.maxHealth = Mathf.FloorToInt(enemy.maxHealth * 0.70f);
                CharacterManager.Instance.cards.ModifyCardCap(-3);
                break;
            case "OverloadedT1":
                CharacterManager.Instance.cards.ModifyCardCap(+2);
                CharacterManager.Instance.health.SetDamageTakenMultiplier(1.5f);
                break;
            case "OverloadedT2":
                CharacterManager.Instance.cards.ModifyCardCap(+4);
                CharacterManager.Instance.health.SetDamageTakenMultiplier(1.75f);
                break;
            case "OverloadedT3":
                CharacterManager.Instance.cards.ModifyCardCap(+6);
                CharacterManager.Instance.health.SetDamageTakenMultiplier(2f);
                break;
            case "ArenaVisionT1":
                CameraManager.Instance.SetCameraZoom(9);
                UIManager.Instance.ShowBlockersUpTo(1);
                break;
            case "ArenaVisionT2":
                CameraManager.Instance.SetCameraZoom(10);
                UIManager.Instance.ShowBlockersUpTo(2);
                break;
            case "ArenaVisionT3":
                CameraManager.Instance.SetCameraZoom(11);
                UIManager.Instance.ShowBlockersUpTo(3);
                break;
            case "TacticalOverflowT1":
                CardDraftManager.Instance.ModifyTacticalOverflow(+1, 1);
                break;
            case "TacticalOverflowT2":
                CardDraftManager.Instance.ModifyTacticalOverflow(+2, 2);
                break;
            case "TacticalOverflowT3":
                CardDraftManager.Instance.ModifyTacticalOverflow(+3, 99);
                break;

            case "TimeWarpedT1":
                ApplyTimeWarpedEffect(0.10f);
                break;
            case "TimeWarpedT2":
                ApplyTimeWarpedEffect(0.20f);
                break;
            case "TimeWarpedT3":
                ApplyTimeWarpedEffect(0.30f);
                break;

            case "CritOnlyT1":
                CharacterManager.Instance.stats.EnableCritOnlyMode(2.5f);
                break;
            case "CritOnlyT2":
                CharacterManager.Instance.stats.EnableCritOnlyMode(3.0f);
                break;
            case "CritOnlyT3":
                CharacterManager.Instance.stats.EnableCritOnlyMode(3.5f);
                break;

            case "HyperModeT1":
            case "HyperModeT2":
            case "HyperModeT3":
                break;

           case "TemporalFluxT1":
                ApplyTemporalFlux(0.05f, 5f, 10f, 0.5f);
                break;
            case "TemporalFluxT2":
                ApplyTemporalFlux(0.10f, 4f, 8f, 0.75f);
                break;
            case "TemporalFluxT3":
                ApplyTemporalFlux(0.15f, 3f, 6f, 1f);
                break;

            default:
                Debug.LogWarning($"Unknown special effect ID: {tier.SpecialEffectID}");
                break;

        }
    }

    private static IEnumerator ApplyTimedInvincibility(Enemy enemy, float duration)
    {
        while (enemy != null)
        {
            yield return new WaitForSeconds(5f);
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
                movement.moveSpeed *= 0.85f;

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

        BioChargeOverTime(enemy, damageGainPerTick);
    }

    private static IEnumerator BioChargeOverTime(Enemy enemy, float gainPerTick)
    {
        float interval = 5f;
        EnemyModifierHandler handler = enemy.GetComponent<EnemyModifierHandler>();
        GameObject handlerGO = handler != null ? handler.gameObject : null;

        while (handler != null && handlerGO != null)
        {
            yield return new WaitForSeconds(interval);

            if (handler == null || handlerGO == null)
                yield break;

            handler.ModifyDamage(gainPerTick);
        }
    }

    private static void ApplyTimeWarpedEffect(float percent)
    {
        if (CharacterManager.Instance == null) return;

        float cooldownScale = 1f - percent;
        float dashPenalty = 1f + percent;

        // Apply dash cooldown modifier
        var ability = CharacterManager.Instance.GetComponent<CharacterAbility>();
        if (ability != null)
            ability.ApplyDashCooldownModifier(dashPenalty);

        // Apply card cooldown modifier
        var cardEffectManager = CharacterManager.Instance.GetComponent<CardEffectManager>();
        if (cardEffectManager != null)
            cardEffectManager.SetGlobalCooldownMultiplier(cooldownScale);
    }
    
    private static IEnumerator ApplyTemporalFlux(float enemyChance, float enemyInterval, float playerInterval, float playerFreezeDuration)
    {
        float enemyTimer = 0f;
        float playerTimer = 0f;

        while (true)
        {
            enemyTimer += Time.deltaTime;
            playerTimer += Time.deltaTime;

            if (enemyTimer >= enemyInterval)
            {
                enemyTimer = 0f;
                foreach (Enemy enemy in Object.FindObjectsByType<Enemy>(FindObjectsSortMode.None))
                {
                    if (enemy != null && Random.value < enemyChance)
                    {
                        EnemyStatus status = enemy.GetComponent<EnemyStatus>();
                        if (status != null)
                        {
                            StatusEffect pause = new StatusEffect(StatusEffectType.Stun, playerFreezeDuration); // reuse stun effect
                            status.ApplyEffect(pause);
                        }
                    }
                }
            }

            if (playerTimer >= playerInterval)
            {
                playerTimer = 0f;
                if (CharacterManager.Instance?.controller != null)
                {
                    CharacterManager.Instance.controller.FreezePlayerFor(playerFreezeDuration);
                }
            }

            yield return null;
        }
    }




}