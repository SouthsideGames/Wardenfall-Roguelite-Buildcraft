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

        foreach (Enemy enemy in damagedEnemies)
        {
            int finalDamage = GetDamage(out bool isCriticalHit);
            enemy.TakeDamage(finalDamage, isCriticalHit);

            EnemyStatus status = enemy.GetComponent<EnemyStatus>();
            if (status != null)
            {
                StatusEffect drainEffect = new StatusEffect(StatusEffectType.Drain, drainDuration, drainDamage, drainInterval);
                status.ApplyEffect(drainEffect);
            }
        }
    }

    protected override void StartAttack()
    {
        base.StartAttack();
        damagedEnemies.Clear();
    }

    protected override void EndAttack()
    {
        base.EndAttack();
    }
}