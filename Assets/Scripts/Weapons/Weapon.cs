using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public abstract class Weapon : MonoBehaviour, IStats
{
    [field: SerializeField] public WeaponDataSO WeaponData {get; private set;}  

    [Header("ELEMENTS:")]
    [SerializeField] protected float range;
    [SerializeField] protected LayerMask enemyMask;


    [Header("SETTINGS:")]
    [SerializeField] protected int damage;
    [SerializeField] protected float attackDelay;
    protected float attackTimer;
    protected int criticalChance;
    protected float criticalPercent;

    
    [Header("ANIMATIONS:")]
    [SerializeField] protected Animator anim;
    [SerializeField] protected float aimLerp;

    [Header("LEVELS:")]
    public int Level { get; private set; }

    protected Enemy closestEnemy;
    protected Vector2 targetUpVector;

    protected Enemy GetClosestEnemy()
    {
        Enemy closestEnemy = null;
        Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, range, enemyMask);

        if(enemies.Length <= 0)
            return null;

        float minDistance = range;

        for (int i = 0; i < enemies.Length; i++)
        {
            Enemy enemyChecked = enemies[i].GetComponent<Enemy>();    

            float distanceToEnemy = Vector2.Distance(transform.position, enemyChecked.transform.position);  

            if(distanceToEnemy < minDistance)
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

        if(Random.Range(0, 101) <= criticalChance)
        {
            isCriticalHit = true;
            return Mathf.RoundToInt(damage * criticalPercent);
        }

        return damage;
    }

    protected virtual void AutoAim()
    {
        closestEnemy = GetClosestEnemy();

        targetUpVector = Vector3.up;

    }

    private void OnDrawGizmos()
    {

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, range);
    }

    public abstract void UpdateStats(StatsManager _statsManager);

    protected void ConfigureStats()
    {
        float multiplier = 1 + (float)Level / 3;

        damage = Mathf.RoundToInt(WeaponData.GetStatValue(Stat.Attack));
        attackDelay = 1f / (WeaponData.GetStatValue(Stat.AttackSpeed) * multiplier);

        criticalChance = Mathf.RoundToInt(WeaponData.GetStatValue(Stat.CriticalChance) * multiplier);
        criticalPercent = WeaponData.GetStatValue(Stat.CriticalPercent) * multiplier;

        if(WeaponData.Prefab.GetType() == typeof(RangedWeapon))
            range = WeaponData.GetStatValue(Stat.Range) * multiplier;   
    }

    public void UpgradeTo(int _targetLevel)
    {
        Level = _targetLevel;
        ConfigureStats();
    }
}
