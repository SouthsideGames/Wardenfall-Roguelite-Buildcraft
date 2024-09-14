using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Pool;

public class RangedEnemyAttack : MonoBehaviour
{
    [Header("Elements")]
    [SerializeField] private Transform firePoint;
    [SerializeField] private EnemyBullet bulletPrefab;
    private PlayerManager player;


    [Header("Setting")]
    [SerializeField] private int damage;
    [SerializeField] private float fireRate;
    [SerializeField] private float attackDelay;
    [SerializeField] private float bulletSpeed;
    private float attackTimer;

    [Header("Type")]
    [SerializeField] private int shooterType;

    [Header("Pool")]
    private ObjectPool<EnemyBullet> bulletPool;

    // Start is called before the first frame update
    void Start()
    {
        attackDelay = 1f / fireRate;

        RangeEnemyType();

        bulletPool = new ObjectPool<EnemyBullet>(CreateFunction, ActionOnGet, ActionOnRelease, ActionOnDestroy);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void RangeEnemyType()
    {
        switch(shooterType)
        {
            case 0:
                attackTimer = attackDelay;
                break;
            case 1:
                attackTimer = 0f;
                break;
            default:
                break;
        }
    }

    public void StorePlayer(PlayerManager _player) =>  player = _player;

    public void AutoAim() => ShootLogic();
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
        //bullet direction
        Vector2 direction = (player.GetColliderCenter() - (Vector2)firePoint.position).normalized;

        EnemyBullet bullet = bulletPool.Get();
        bullet.Shoot(damage, direction);
    }

    public void ReleaseBullet(EnemyBullet _bullet) => bulletPool.Release(_bullet);

#region POOLING
    private EnemyBullet CreateFunction()
    {
        EnemyBullet bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        bullet.Configure(this);
        return bullet;
    }

    private void ActionOnGet(EnemyBullet _enemyBullet)
    {   
        _enemyBullet.Reload();
        _enemyBullet.transform.position = firePoint.position;
        _enemyBullet.gameObject.SetActive(true);
    }

    private void ActionOnRelease(EnemyBullet _enemyBullet) =>  _enemyBullet.gameObject.SetActive(false);
    private void ActionOnDestroy(EnemyBullet _enemyBullet) => Destroy(_enemyBullet.gameObject);

#endregion

}
