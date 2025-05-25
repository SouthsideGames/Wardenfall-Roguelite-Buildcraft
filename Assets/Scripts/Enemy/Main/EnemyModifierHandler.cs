using UnityEngine;

[RequireComponent(typeof(Enemy))]
public class EnemyModifierHandler : MonoBehaviour
{
    private Enemy enemy;
    private EnemyMovement movement;

    [SerializeField] private bool canPerformCriticalHit;
    private float damageMultiplier = 1f;
    private float critChanceModifier = 1f;

    private void Awake()
    {
        enemy = GetComponent<Enemy>();
        movement = GetComponent<EnemyMovement>();
    }

    public void ApplyTraits()
    {
        if (TraitManager.Instance == null) return;

        foreach (var (trait, stack) in TraitManager.Instance.GetAllActiveTraits())
        {
            if (trait == null) continue;

            TraitTier tier = trait.GetTier(stack);
            if (tier == null) continue;

            ApplyTierEffects(tier, stack);
        }
    }

    private void ApplyTierEffects(TraitTier tier, int stack)
    {
        if (movement != null)
            movement.moveSpeed *= 1f + tier.SpeedModifier;

        if (!string.IsNullOrEmpty(tier.SpecialEffectID))
            TraitEffectUtils.ApplySpecialEffect(enemy, tier, stack);
    }

    public void ModifyDamage(float modifier)
    {
        if (this == null || gameObject == null)
            return;

        damageMultiplier += modifier;
    }

    public void ModifyCritChance(float multiplier)
    {
        critChanceModifier = multiplier;
        Debug.Log($"{gameObject.name} crit chance modifier set to {critChanceModifier:F2}");
    }

    public void ModifySpeed(float modifier)
    {
        if (this == null || gameObject == null || movement == null)
            return;

        movement.moveSpeed *= (1f + modifier);
    }

    public float GetDamageMultiplier() => damageMultiplier;
    public float GetCritChanceModifier() => critChanceModifier;
    public bool CanCrit() => canPerformCriticalHit;
}
