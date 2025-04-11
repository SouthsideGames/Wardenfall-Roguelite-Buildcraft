using UnityEngine;

public static class EnemyTraitApplier
{
    public static void ApplyTraitsTo(Enemy enemy)
    {
        EnemyMovement movement = enemy.GetComponent<EnemyMovement>();

        foreach (var (trait, stack) in EnemyTraitManager.Instance.GetAllActiveTraits())
        {
            var tier = trait.GetTier(stack);

            if (movement != null)
                movement.moveSpeed *= 1f + tier.SpeedModifier;

            if (!string.IsNullOrEmpty(tier.SpecialEffectID))
                TraitEffectUtils.ApplySpecialEffect(enemy, tier, stack);
        }
    }
}
