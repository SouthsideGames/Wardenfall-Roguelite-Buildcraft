using UnityEngine;

public class BulletBase : MonoBehaviour
{
    [Header("BULLET SETTINGS")]
    [SerializeField] protected float moveSpeed = 10f;
    [SerializeField] protected float lifetime = 3f;
    [SerializeField] protected LayerMask enemyMask;

    protected int damage;
    protected bool isCriticalHit;
    protected Rigidbody2D rb;
    private Collider2D col;
    protected RangedWeapon rangedWeapon;
    private Enemy target;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
    }

    public void Configure(RangedWeapon _rangedWeapon)
    {
        this.rangedWeapon = _rangedWeapon;
    }


    public virtual void Shoot(int _damage, Vector2 direction, bool _isCriticalHit)
    {
        damage = _damage;
        isCriticalHit = _isCriticalHit;

        rb.linearVelocity = direction.normalized * moveSpeed;
        Invoke(nameof(DestroyBullet), lifetime);
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (target != null)
            return;

        if (IsInLayerMask(collision.gameObject.layer, enemyMask))
        {
            target = collision.GetComponent<Enemy>();

            if (target != null)
            {
                CancelInvoke();
                ApplyDamage(target);
                Release();
            }
        }
    }

    protected virtual void ApplyDamage(Enemy enemy)
    {
        enemy.TakeDamage(damage, isCriticalHit);
    }

    protected virtual void DestroyBullet()
    {
        gameObject.SetActive(false);
    }

    protected bool IsInLayerMask(int layer, LayerMask layerMask)
    {
        return (layerMask.value & (1 << layer)) != 0;
    }

    protected virtual void Release()
    {
        if (!gameObject.activeSelf)
            return;

        rangedWeapon.ReleaseBullet(this);
    }

    public void Reload()
    {
        target = null;

        rb.linearVelocity = Vector2.zero;
        col.enabled = true;

    }

}
