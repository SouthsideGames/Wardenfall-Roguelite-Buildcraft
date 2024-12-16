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
    [SerializeField] private BulletBase bulletPrefab;

    [Header("POOL:")]
    public ObjectPool<BulletBase> bulletPool {get; private set;}

    void Start() =>  bulletPool = new ObjectPool<BulletBase>(CreateFunction, ActionOnGet, ActionOnRelease, ActionOnDestroy);
    void Update() => AutoAimLogic();
    
    protected override void AutoAimLogic()
    {
        base.AutoAimLogic();

        if(closestEnemy != null)
        {
            targetUpVector = (closestEnemy.GetCenter() - (Vector2)transform.position).normalized;
            transform.up = targetUpVector;  
           
            ShootLogic();
            return;
        }

        transform.up = Vector3.Lerp(transform.up, targetUpVector, Time.deltaTime * aimLerp);
    }

    protected void ShootLogic()
    {
        attackTimer += Time.deltaTime;

        if(attackTimer > attackDelay)
        {
            attackTimer = 0f;
            Shoot();
        }
    }

    protected virtual void Shoot()
    {
        OnBulletFired?.Invoke();
        anim.Play("Attack");

        int damage = GetDamage(out bool isCriticalHit);

        BulletBase _bullet = bulletPool.Get();
        _bullet.Shoot(damage, transform.up, isCriticalHit);

        PlaySFX();
    }

    #region POOLING
    private BulletBase CreateFunction()
    {
        BulletBase bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        bullet.Configure(this);
        return bullet;
    }

    private void ActionOnGet(BulletBase _bullet)
    {   
        _bullet.Reload();
        _bullet.transform.position = firePoint.position;
        _bullet.gameObject.SetActive(true);
    }

    private void ActionOnRelease(BulletBase _bullet) => _bullet.gameObject.SetActive(false);
    private void ActionOnDestroy(BulletBase _bullet) => Destroy(_bullet.gameObject);
    public void ReleaseBullet(BulletBase _bullet) => bulletPool.Release(_bullet);

    #endregion

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
