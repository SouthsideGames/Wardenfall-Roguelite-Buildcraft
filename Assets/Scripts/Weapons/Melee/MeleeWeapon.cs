using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeWeapon : Weapon
{
    [HideInInspector] public MeleeWeaponState state;

    [Header("ELEMENTS:")]
    [SerializeField] private Transform hitpoint;
    [SerializeField] public BoxCollider2D hitCollider;
    [HideInInspector] public List<Enemy> damagedEnemies = new List<Enemy>();

    private void Awake()
    {
        WaveManager.OnWaveCompleted  += OnWaveCompletedCallback;
        CharacterHealth.OnCharacterDeath += OnWaveCompletedCallback;
    }

    private void OnDestroy()
    {
        WaveManager.OnWaveCompleted  -= OnWaveCompletedCallback;
        CharacterHealth.OnCharacterDeath -= OnWaveCompletedCallback;
    }


    void Start() => state = MeleeWeaponState.Idle;

    void Update()
    {
        switch (state)
        {
            case MeleeWeaponState.Idle:
                if (AutoAim)
                    AutoAimLogic();
                else
                    TimerAttackLogic();
                break;
            case MeleeWeaponState.Attack:
                AttackState();
                break;
            case MeleeWeaponState.Empty:
                break;
            default:
                break;
        }
    }

    protected override void StartAttack()
    {
        anim.Play("Attack");
        state = MeleeWeaponState.Attack;

        damagedEnemies.Clear();
        anim.speed = 1f / attackDelay;
    }

    private void EndAttack()
    {
        state = MeleeWeaponState.Idle;
        damagedEnemies.Clear();
    }

    private void AttackState() => AttackLogic();
    protected virtual void AttackLogic()
    {
        Collider2D[] enemies = Physics2D.OverlapBoxAll(
            hitpoint.position,
            hitCollider.bounds.size,
            hitpoint.localEulerAngles.z,
            enemyMask);

        for (int i = 0; i < enemies.Length; i++)
        {
            Enemy enemy = enemies[i].GetComponent<Enemy>();

            if (!damagedEnemies.Contains(enemy))
            {
                int damage = GetDamage(out bool isCriticalHit);

                enemy.TakeDamage(damage, isCriticalHit);
                damagedEnemies.Add(enemy);
            }
        }
    }

    public override void UpdateWeaponStats(CharacterStats _statsManager)
    {
        ConfigureWeaponStats();

        damage = Mathf.RoundToInt(damage * (1 + _statsManager.GetStatValue(Stat.Attack) / 100));
        attackDelay /= 1 + (_statsManager.GetStatValue(Stat.AttackSpeed) / 100);

        criticalHitChance = Mathf.RoundToInt(criticalHitChance * (1 + _statsManager.GetStatValue(Stat.CritChance) / 100));
        criticalHitDamageAmount += _statsManager.GetStatValue(Stat.CritDamage);
    }

    private void OnWaveCompletedCallback() => state = MeleeWeaponState.Empty;

}
