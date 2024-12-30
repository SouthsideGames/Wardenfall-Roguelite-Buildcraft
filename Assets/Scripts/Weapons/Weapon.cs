using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public abstract class Weapon : MonoBehaviour, IStats
{
    [field: SerializeField] public WeaponDataSO WeaponData { get; private set; }

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

    private void Update()
    {
        if (useAutoAim)
        {
            AutoAimLogic();
        }
        else
        {
            ManualAttackLogic();
        }
    }

    protected void PlaySFX()
    {
        audioSource.pitch = Random.Range(0.95f, 1.05f);
        audioSource.Play();
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

        if (Random.Range(0, 101) <= criticalHitChance)
        {
            isCriticalHit = true;
            return Mathf.RoundToInt(damage * criticalHitDamageAmount);
        }

        return damage;
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
    

    protected abstract void Attack();

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
}