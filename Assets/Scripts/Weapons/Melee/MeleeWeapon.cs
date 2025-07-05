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

    protected override void OnEnable()
    {
        base.OnEnable();
    }

    protected override void OnDisable()
    {
        base.OnDisable();
    }

    protected override void HandleGameStateChanged(GameState newState)
    {
        base.HandleGameStateChanged(newState);

        if (!isGameplayActive && anim != null)
        {
            anim.Play("Idle");
        }
    }

    void Start()
    {
        state = MeleeWeaponState.Idle;
    }

    void Update()
    {
        if (!isGameplayActive) return;
        Attack();
    }

    public override void Attack()
    {
        if (!isGameplayActive) return;

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

    private void AttackState() => AttackLogic();

    protected virtual void StartAttack()
    {
        if (!isGameplayActive) return;

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
        if (!isGameplayActive) return;

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
        if (!isGameplayActive) return;

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
        if (!isGameplayActive) return;

        if (attackTimer >= attackDelay)
        {
            attackTimer = 0;
            StartAttack();
        }
    }

    private void ManageFreeAttackTimer()
    {
        if (!isGameplayActive) return;

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

    private void Wait()
    {
        if (!isGameplayActive) return;
        attackTimer += Time.deltaTime;
    }

    public override void UpdateWeaponStats(CharacterStats _statsManager)
    {
        ConfigureWeaponStats();

        damage = Mathf.RoundToInt(damage * (1 + _statsManager.GetStatValue(Stat.Attack) / 100));
        attackDelay /= 1 + (_statsManager.GetStatValue(Stat.AttackSpeed) / 100);

        criticalHitChance = Mathf.RoundToInt(criticalHitChance * 91 + _statsManager.GetStatValue(Stat.CritChance) / 100);
        criticalHitDamageAmount += _statsManager.GetStatValue(Stat.CritDamage);
    }

    private void HitStop(bool criticalHit)
    {
        if (criticalHit)
        {
            Time.timeScale = 0.1f;
            Invoke("ResumeTime", 0.2f); 
        }
    }

    private void ResumeTime()
    {
        Time.timeScale = 1f;
    }
}
