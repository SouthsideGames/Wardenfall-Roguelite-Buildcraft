using System.Collections;
using UnityEngine;

public static class TraitEffectUtils
{
    public static void ApplySpecialEffect(Enemy enemy, EnemyTraitTier tier, int stackCount)
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
}
