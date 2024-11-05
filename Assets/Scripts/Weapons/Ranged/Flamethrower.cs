using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flamethrower : Weapon
{
    [Header("FLAMETHROWER SPECIFICS:")]
    [SerializeField] private float flameRange;
    [SerializeField] private float flameAngle;

    void Update()
    {
        AutoAim();
        if (attackTimer >= attackDelay)
        {
            EmitFlame();
            attackTimer = 0f;
        }
        attackTimer += Time.deltaTime;
    }

    private void EmitFlame()
    {
        Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, flameRange, enemyMask);
        foreach (var enemy in enemies)
        {
            Vector2 direction = (enemy.transform.position - transform.position).normalized;
            float angle = Vector2.Angle(transform.up, direction);
            if (angle < flameAngle / 2)
            {
                enemy.GetComponent<Enemy>().TakeDamage((int)(damage * Time.deltaTime), _isCriticalHit: false);
            }
        }
    }

    public override void UpdateWeaponStats(CharacterStats _statsManager)
    {
        ConfigureWeaponStats();

        damage = Mathf.RoundToInt(damage * (1 + _statsManager.GetStatValue(Stat.Attack) / 100));
        attackDelay  /= 1 + (_statsManager.GetStatValue(Stat.AttackSpeed) / 100); 

        criticalHitChance = Mathf.RoundToInt(criticalHitChance * 91 + _statsManager.GetStatValue(Stat.CritChance) / 100); 
        criticalHitDamageAmount += _statsManager.GetStatValue(Stat.CritDamage); //Deal times additional damage

        range += _statsManager.GetStatValue(Stat.Range) / 10;

    }

}
