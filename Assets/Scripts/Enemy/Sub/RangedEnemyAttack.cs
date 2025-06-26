using UnityEngine;
using UnityEngine.Pool;

public class RangedEnemyAttack : MonoBehaviour
{
    [Header("ELEMENTS:")]
    [SerializeField] private Transform firePoint;
    [SerializeField] private EnemyBullet bulletPrefab;
    private CharacterManager player;


    [Header("SETTINGS:")]
    [SerializeField] private int bulletDamage;
    [SerializeField] private float fireRate;
    [SerializeField] private float attackDelay;
    [SerializeField] private float bulletSpeed;
    private float attackTimer;

    private ObjectPool<EnemyBullet> bulletPool;

    void Start()
    {
        attackDelay = 1f / fireRate;

        attackTimer = attackDelay;

        bulletPool = new ObjectPool<EnemyBullet>(CreateFunction, ActionOnGet, ActionOnRelease, ActionOnDestroy);
    }

    public void StorePlayer(CharacterManager _player) =>  player = _player;

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
        
        Vector2 direction = (player.GetColliderCenter() - (Vector2)firePoint.position).normalized;
        InstantShoot(direction);
 
    }

    public void InstantShoot(Vector2 direction)
    {
        EnemyBullet bullet = bulletPool.Get();
        bullet.Shoot(bulletDamage, direction);
    }

    public void ReleaseBullet(EnemyBullet _bullet) => bulletPool.Release(_bullet);

    public void FireCustomDirection(Vector2 direction, float speed)
    {
        if (bulletPrefab == null) return;

        EnemyBullet bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        if (rb != null)
            rb.linearVelocity = direction.normalized * speed;

        // Optional: rotate bullet sprite to face direction
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        bullet.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }


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
