using UnityEngine;

public class StunMeleeWeapon : MeleeWeapon
{
    [Header("STUN SETTINGS:")]
    [SerializeField] private float stunDuration = 2.0f;

    protected override void AttackLogic()
    {
        base.AttackLogic();

        foreach (Enemy enemy in damagedEnemies)
        {
            int finalDamage = GetDamage(out bool isCriticalHit);
            enemy.TakeDamage(finalDamage, isCriticalHit);

            EnemyStatus status = enemy.GetComponent<EnemyStatus>();
            if (status != null && !status.IsStunned)
            {
                StatusEffect stunEffect = new StatusEffect(StatusEffectType.Stun, stunDuration);
                status.ApplyEffect(stunEffect);
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
