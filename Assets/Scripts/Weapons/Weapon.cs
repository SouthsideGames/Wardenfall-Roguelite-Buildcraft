using System;
using System.Collections;
using System.Collections.Generic;
using SouthsideGames.DailyMissions;
using Unity.VisualScripting;
using UnityEngine;

public abstract class Weapon : MonoBehaviour, IStats, IWeaponSystem
{
    [field: SerializeField] public WeaponDataSO WeaponData { get; private set; }

    public static Action OnCriticalHit;

    [Header("ELEMENTS:")]
    protected float range;
    [SerializeField] protected LayerMask enemyMask;

    [Header("SETTINGS:")]
    protected int damage;
    protected float attackDelay;
    protected float attackTimer;
    protected int criticalHitChance;
    protected float criticalHitDamageAmount;
    [SerializeField] protected bool useAutoAim = true;

    [Header("ANIMATIONS:")]
    [SerializeField] protected Animator anim;
    [SerializeField] protected float aimLerp;

    [Header("LEVELS:")]
    public int Level { get; private set; }

    protected Enemy closestEnemy;
    protected Vector2 targetUpVector;
    protected AudioSource audioSource;

    private void Awake()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.clip = WeaponData.AttackSound;
    }


    protected void PlaySFX()
    {
        if (audioSource != null && WeaponData.AttackSound != null)
        {
            audioSource.pitch = UnityEngine.Random.Range(0.95f, 1.05f);
            audioSource.Play();
        }
    }

    protected Enemy GetClosestEnemy()
    {
        Enemy closestEnemy = null;
        Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, range, enemyMask);

        if (enemies.Length <= 0)
            return null;

        float minDistance = range;

        for (int i = 0; i < enemies.Length; i++)
        {
            Enemy enemyChecked = enemies[i].GetComponent<Enemy>();

            float distanceToEnemy = Vector2.Distance(transform.position, enemyChecked.transform.position);

            if (distanceToEnemy < minDistance)
            {
                closestEnemy = enemyChecked;
                minDistance = distanceToEnemy;
            }
        }

        return closestEnemy;
    }

   protected int GetDamage(out bool isCriticalHit)
    {
        isCriticalHit = false;
        int finalDamage = damage;

        if (UnityEngine.Random.Range(0, 101) <= criticalHitChance)
        {
            isCriticalHit = true;
            MissionManager.Increment(MissionType.criticalHitMastery, 1);
            finalDamage = Mathf.RoundToInt(damage * criticalHitDamageAmount);
            AudioManager.Instance.PlayCrowdReaction(CrowdReactionType.Gasp);
            WaveManager.Instance?.AdjustViewerScore(0.03f);
            OnCriticalHit?.Invoke();
        }

        StatisticsManager.Instance.RecordWeaponUsage(WeaponData.ID, finalDamage);
        return finalDamage;
    }

    protected virtual void AutoAimLogic()
    {
        if (!useAutoAim) return;

        closestEnemy = GetClosestEnemy();
        targetUpVector = Vector3.up;
    }

    protected virtual void ManualAttackLogic()
    {
        attackTimer += Time.deltaTime;

        if (attackTimer >= attackDelay)
        {
            attackTimer = 0;
            Attack();
        }
    }
    

    public abstract void Attack();

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, range);
    }

    public abstract void UpdateWeaponStats(CharacterStats _statsManager);

    protected void ConfigureWeaponStats()
    {
        Dictionary<Stat, float> calculatedStats = WeaponStatCalculator.GetStats(WeaponData, Level);

        damage = Mathf.RoundToInt(calculatedStats[Stat.Attack]);
        attackDelay = 1f / calculatedStats[Stat.AttackSpeed];
        criticalHitChance = Mathf.RoundToInt(calculatedStats[Stat.CritChance]);
        criticalHitDamageAmount = calculatedStats[Stat.CritDamage];
        range = calculatedStats[Stat.Range];

        anim.speed = 1f / attackDelay;
    }

    public void UpgradeWeaponTo(int _targetLevel)
    {
        Level = _targetLevel;
        ConfigureWeaponStats();
    }

    public int GetWeaponRecyclePrice() => WeaponStatCalculator.GetRecyclePrice(WeaponData, Level);

    public void Upgrade() => UpgradeWeaponTo(Level + 1);

    public void Upgrade(float damageMultiplier)
    {
        damage = Mathf.RoundToInt(damage * damageMultiplier);
        ConfigureWeaponStats();
    }

    public void SetWeapon(WeaponDataSO weaponData)
    {
        WeaponData = weaponData;
        ConfigureWeaponStats();
    }

    public float GetDamage() => damage;
    public float GetAttackSpeed() => 1f / attackDelay;
    
    public virtual bool CanAttack() => attackTimer >= attackDelay;
    
    public virtual void OnWeaponEquipped()
    {
        gameObject.SetActive(true);
        ConfigureWeaponStats();
    }

    public virtual void OnWeaponUnequipped()
    {
        gameObject.SetActive(false);
        ResetWeaponStats();
    }

    public bool IsWeaponActive() => gameObject.activeInHierarchy;

    public void ResetWeaponStats()
    {
        attackTimer = 0;
        damage = 0;
        attackDelay = 0;
        criticalHitChance = 0;
        criticalHitDamageAmount = 0;
        range = 0;
    }

    public float GetUpgradeCost() => WeaponStatCalculator.GetPurchasePrice(WeaponData, Level + 1);

    public string GetWeaponDescription() => WeaponData.Description;
    
    public WeaponDataSO GetWeaponData() => WeaponData;
}