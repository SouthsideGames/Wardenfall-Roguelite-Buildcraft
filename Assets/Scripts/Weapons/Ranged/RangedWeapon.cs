 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using System;

public class RangedWeapon : Weapon
{
    public static Action OnBulletFired;

    [Header("ELEMENTS:")]
    [SerializeField] private Transform firePoint;
    [SerializeField] private Bullet bulletPrefab;

    [Header("POOL:")]
    public ObjectPool<Bullet> bulletPool {get; private set;}

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

    protected virtual void Shoot()
    {
        OnBulletFired?.Invoke();

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
