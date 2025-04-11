using UnityEngine;

public static class EnemyTraitApplier
{
    public static void ApplyTraitsTo(Enemy enemy)
    {
        // Safety check in case the manager isn't ready yet
        if (EnemyTraitManager.Instance == null)
        {
            Debug.LogWarning("EnemyTraitManager is not initialized yet.");
            return;
        }

        EnemyMovement movement = enemy.GetComponent<EnemyMovement>();

        foreach (var (trait, stack) in EnemyTraitManager.Instance.GetAllActiveTraits())
        {
            if (trait == null) continue;

            var tier = trait.GetTier(stack);
            if (tier == null) continue;

            // Apply speed modifier (e.g., for Armored)
            if (movement != null)
                movement.moveSpeed *= 1f + tier.SpeedModifier;

            // Run special effects (like invincibility)
            if (!string.IsNullOrEmpty(tier.SpecialEffectID))
                TraitEffectUtils.ApplySpecialEffect(enemy, tier, stack);
        }
    }
}
