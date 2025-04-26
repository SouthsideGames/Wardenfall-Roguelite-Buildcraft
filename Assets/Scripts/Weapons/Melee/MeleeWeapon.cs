using System;
using System.Collections;
using System.Collections.Generic;
using SouthsideGames.DailyMissions;
using UnityEngine;

public class MeleeWeapon : Weapon
{
    private MeleeWeaponState state;

    [Header("ELEMENTS:")]
    public Transform hitpoint;
    public BoxCollider2D hitCollider;
    public List<Enemy> damagedEnemies = new List<Enemy>();

    void Start()
    {
        state = MeleeWeaponState.Idle;
    }

    void Update()
    {
        Attack();
    }

    public override void Attack()
    {
        if (useAutoAim)
        {
            switch (state)
            {
                case MeleeWeaponState.Idle:
                    AutoAimLogic();
                    break;
                case MeleeWeaponState.Attack:
                    AttackState();
                    break;
            }
        }
        else
        {
            ManageFreeAttackTimer();
        }
            
        
    }

    private void AttackState()
    {
        AttackLogic();
    }

    protected virtual void StartAttack()
    {
        anim.Play("Attack");
        state = MeleeWeaponState.Attack;

        damagedEnemies.Clear();

        anim.speed = 1f / attackDelay;
    }

    protected virtual void EndAttack()
    {
        state = MeleeWeaponState.Idle;
        damagedEnemies.Clear();
    }

    protected virtual void AttackLogic()
    {
        Collider2D[] enemies = Physics2D.OverlapBoxAll
        (
            hitpoint.position,
            hitCollider.bounds.size,
            hitpoint.localEulerAngles.z,
            enemyMask
        );

        foreach (Collider2D collider in enemies)
        {
            Enemy enemy = collider.GetComponent<Enemy>();
            if (!damagedEnemies.Contains(enemy))
            {
                int damage = GetDamage(out bool isCriticalHit);
                enemy.TakeDamage(damage, isCriticalHit);
                if (isCriticalHit)
                {
                    HitStop(true);
                }
                damagedEnemies.Add(enemy);
                MissionManager.Increment(MissionType.weaponSpecialist, damage);
            }
        }
    }

    protected override void AutoAimLogic()
    {
        base.AutoAimLogic();

        if (closestEnemy != null)
        {
            targetUpVector = (closestEnemy.transform.position - transform.position).normalized;
            transform.up = targetUpVector;

            if (targetUpVector.x < 0)
                transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            else
                transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);

            ManageAttackTimer();
        }

        transform.up = Vector3.Lerp(transform.up, targetUpVector, Time.deltaTime * aimLerp);

        Wait();
    }

    private void ManageAttackTimer()
    {
        if (attackTimer >= attackDelay)
        {
            attackTimer = 0;
            StartAttack();
        }
    }

    private void ManageFreeAttackTimer()
    {
        AttackLogic();

        if (attackTimer >= attackDelay)
        {
            attackTimer = 0;
            StartAttack();
        }
        else
        {
            Wait();
        }
    }

    private void Wait() => attackTimer += Time.deltaTime;

    public override void UpdateWeaponStats(CharacterStats _statsManager)
    {
        ConfigureWeaponStats();

        damage = Mathf.RoundToInt(damage * (1 + _statsManager.GetStatValue(Stat.Attack) / 100));
        attackDelay /= 1 + (_statsManager.GetStatValue(Stat.AttackSpeed) / 100);

        criticalHitChance = Mathf.RoundToInt(criticalHitChance * 91 + _statsManager.GetStatValue(Stat.CritChance) / 100);
        criticalHitDamageAmount += _statsManager.GetStatValue(Stat.CritDamage);

    }

    // Added HitStop function -  needs implementation details based on your game's specifics.
    private void HitStop(bool criticalHit)
    {
        // Implement hit stop logic here.  This might involve Time.timeScale manipulation,
        // coroutines, or other techniques to temporarily slow or pause game time.
        // Example (using Time.timeScale):
        if (criticalHit)
        {
            Time.timeScale = 0.1f; // Slows down time
            Invoke("ResumeTime", 0.2f); // Resumes after 0.2 seconds
        }
    }

    private void ResumeTime()
    {
        Time.timeScale = 1f;
    }

}