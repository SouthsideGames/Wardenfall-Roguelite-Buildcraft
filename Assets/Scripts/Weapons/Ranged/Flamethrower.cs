using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flamethrower : Weapon
{
   [Header("FLAMETHROWER SPECIFICS:")]
    [SerializeField] private float flameRange;
    [SerializeField] private float flameAngle;
    private bool attackLeftSide = true; // Toggle to determine which side to attack

    void Update() =>  TimerAttackLogic();

    protected override void TimerAttackLogic()
    {
        base.TimerAttackLogic();
        if (attackTimer == 0) // Trigger attack when the timer resets
        {
            StartAttack();
        }
    }

    protected override void StartAttack()
    {
        EmitFlame();
        attackTimer = attackDelay; // Reset the attack timer
        attackLeftSide = !attackLeftSide; // Toggle the attack side
    }

    private void EmitFlame()
    {
        Vector2 attackDirection = attackLeftSide ? Vector2.left : Vector2.right; // Determine attack side
        Vector2 attackPosition = (Vector2)transform.position + attackDirection * flameRange * 0.5f;

        Collider2D[] enemies = Physics2D.OverlapCircleAll(attackPosition, flameRange, enemyMask);
        foreach (var enemy in enemies)
        {
            Vector2 direction = (enemy.transform.position - transform.position).normalized;
            float angle = Vector2.Angle(transform.up, direction);
            if (angle < flameAngle / 2)
            {
                enemy.GetComponent<Enemy>().TakeDamage((int)(damage * Time.deltaTime), _isCriticalHit: false);
            }
        }

        // Debug visuals for flame emission
        Debug.DrawLine(transform.position, attackPosition, Color.red, 0.1f);
    }

    public override void UpdateWeaponStats(CharacterStats _statsManager)
    {
        ConfigureWeaponStats();

        damage = Mathf.RoundToInt(damage * (1 + _statsManager.GetStatValue(Stat.Attack) / 100));
        attackDelay /= 1 + (_statsManager.GetStatValue(Stat.AttackSpeed) / 100);

        criticalHitChance = Mathf.RoundToInt(criticalHitChance * (1 + _statsManager.GetStatValue(Stat.CritChance) / 100));
        criticalHitDamageAmount += _statsManager.GetStatValue(Stat.CritDamage);

        range += _statsManager.GetStatValue(Stat.Range) / 10;
    }
}
