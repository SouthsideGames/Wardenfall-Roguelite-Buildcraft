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
            closestEnemy.GetComponent<EnemyStatus>()?.ApplyEffect(StatusEffectType.Drain, drainDamage, drainDuration, drainInterval);
    }
}
