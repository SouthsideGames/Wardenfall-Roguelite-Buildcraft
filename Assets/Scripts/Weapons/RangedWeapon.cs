using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class RangedWeapon : Weapon
{

    [Header("ELEMENTS:")]
    [SerializeField] private Transform firePoint;
    [SerializeField] private Bullet bulletPrefab;

    [Header("POOL:")]
    private ObjectPool<Bullet> bulletPool;

    // Start is called before the first frame update
    void Start()
    {
        bulletPool = new ObjectPool<Bullet>(CreateFunction, ActionOnGet, ActionOnRelease, ActionOnDestroy);
    }

    // Update is called once per frame
    void Update()
    {
        AutoAim();
    }

    protected override void AutoAim()
    {
        base.AutoAim();

        if(closestEnemy != null)
        {
            targetUpVector = (closestEnemy.transform.position - transform.position).normalized;
            transform.up = targetUpVector;  
           
            ShootLogic();
            return;
        }

         transform.up = Vector3.Lerp(transform.up, targetUpVector, Time.deltaTime * aimLerp);

    }

    private void ShootLogic()
    {
        attackTimer += Time.deltaTime;

        if(attackTimer > attackDelay){
            attackTimer = 0f;
            Shoot();
        }
    }

    private void Shoot()
    {
        int damage = GetDamage(out bool isCriticalHit);

        Bullet _bullet = bulletPool.Get();
        _bullet.Shoot(damage, transform.up, isCriticalHit );
    }

    #region POOLING
    private Bullet CreateFunction()
    {
        Bullet bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        bullet.Configure(this);
        return bullet;
    }

    private void ActionOnGet(Bullet _bullet)
    {   
        _bullet.Reload();
        _bullet.transform.position = firePoint.position;
        _bullet.gameObject.SetActive(true);
    }

    private void ActionOnRelease(Bullet _bullet) =>  _bullet.gameObject.SetActive(false);
    private void ActionOnDestroy(Bullet _bullet) => Destroy(_bullet.gameObject);

    public void ReleaseBullet(Bullet _bullet) => bulletPool.Release(_bullet);

    #endregion

    public override void UpdateStats(StatsManager _statsManager)
    {
        ConfigureStats();

        damage = Mathf.RoundToInt(damage * (1 + _statsManager.GetStatValue(Stat.Attack) / 100));
        attackDelay  /= 1 + (_statsManager.GetStatValue(Stat.AttackSpeed) / 100); 

        criticalChance = Mathf.RoundToInt(criticalChance * 91 + _statsManager.GetStatValue(Stat.CriticalChance) / 100); 
        criticalPercent += _statsManager.GetStatValue(Stat.CriticalPercent); //Deal times additional damage

        range += _statsManager.GetStatValue(Stat.Range) / 10;

    }

}
