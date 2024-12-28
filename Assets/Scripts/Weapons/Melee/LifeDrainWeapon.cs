using UnityEngine;

public class LifeDrainWeapon : MeleeWeapon
{
    [Header("DRAIN SETTINGS:")]
    [SerializeField] private float drainDuration = 5.0f;
    [SerializeField] private float drainInterval = 1.0f;
    [SerializeField] private int drainDamage = 2;

    protected override void AttackLogic()
    {
        base.AttackLogic();

        if (closestEnemy != null)
        {
            EnemyStatus status = closestEnemy.GetComponent<EnemyStatus>();
            if (status != null)
            {
                StatusEffect drainEffect = new StatusEffect(StatusEffectType.Drain, drainDuration, drainDamage, drainInterval);
                status.ApplyEffect(drainEffect);
            }
        }
    }
}