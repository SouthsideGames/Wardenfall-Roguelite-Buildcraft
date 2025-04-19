using UnityEngine;

public static class TraitApplier
{
    public static void ApplyTraitsTo(Enemy enemy)
    {
       
        if (TraitManager.Instance == null)
            return;

        EnemyMovement movement = enemy.GetComponent<EnemyMovement>();

        foreach (var (trait, stack) in TraitManager.Instance.GetAllActiveTraits())
        {
            if (trait == null) continue;

            var tier = trait.GetTier(stack);
            if (tier == null) continue;

            if (movement != null)
                movement.moveSpeed *= 1f + tier.SpeedModifier;

            if (!string.IsNullOrEmpty(tier.SpecialEffectID))
                TraitEffectUtils.ApplySpecialEffect(enemy, tier, stack);
        }
    }
}
