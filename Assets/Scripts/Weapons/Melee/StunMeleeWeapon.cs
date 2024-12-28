using UnityEngine;

public class StunMeleeWeapon : MeleeWeapon
{
    [Header("STUN SETTINGS:")]
    [SerializeField] private float stunDuration = 2.0f; // Duration to stun enemies

    protected override void AttackLogic()
    {
        base.AttackLogic();

        // Trigger attack animation
        anim.Play("Attack");

        // Apply damage and stun effect to enemies hit
        if (damagedEnemies.Count > 0)
        {
            foreach (Enemy enemy in damagedEnemies)
            {
                // Deal damage to the enemy
                int finalDamage = GetDamage(out bool isCriticalHit);
                enemy.TakeDamage(finalDamage, isCriticalHit);

                // Apply stun effect using new StatusEffect system
                EnemyStatus status = enemy.GetComponent<EnemyStatus>();
                if (status != null)
                {
                    StatusEffect stunEffect = new StatusEffect(StatusEffectType.Stun, stunDuration);
                    status.ApplyEffect(stunEffect);
                }
            }
        }
    }
}
