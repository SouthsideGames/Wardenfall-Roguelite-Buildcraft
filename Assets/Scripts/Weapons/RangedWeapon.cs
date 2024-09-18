using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class RangedWeapon : Weapon
{

    [Header("Elements")]
    [SerializeField] private Transform firePoint;
    [SerializeField] private Bullet bulletPrefab;

    [Header("Pooling")]
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

}
