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
    protected Enemy target;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
    }

    public void Configure(RangedWeapon _rangedWeapon) => this.rangedWeapon = _rangedWeapon;

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
                ApplyOnHitEffects(target);
                Release();
            }
        }
    }

    protected virtual void ApplyDamage(Enemy enemy)
    {
        int finalDamage = damage;
        
        var cm = CharacterManager.Instance;
        if (cm.cards.HasCard("radiant_core"))
        {
            EnemyStatus status = enemy.GetComponent<EnemyStatus>();
            if (status != null && status.HasAnyEffect())
            {
                finalDamage = Mathf.RoundToInt(damage * 1.1f);
                Debug.Log("Radiant Core triggered: bonus damage applied.");
            }
        }

        enemy.TakeDamage(finalDamage, isCriticalHit);
    }

    protected virtual void ApplyOnHitEffects(Enemy enemy)
    {
        var cm = CharacterManager.Instance;

        if (cm.cards.HasCard("burnwake"))
        {
            float chance = 0.15f;
            if (Random.value < chance)
            {
                var status = enemy.GetComponent<EnemyStatus>();
                if (status != null)
                {
                    StatusEffect burn = new StatusEffect(StatusEffectType.Burn, 3f, 5f, 1f);
                    status.ApplyEffect(burn);
                }
            }
        }
    }

    protected virtual void DestroyBullet() => gameObject.SetActive(false);
    protected bool IsInLayerMask(int layer, LayerMask layerMask) => (layerMask.value & (1 << layer)) != 0;

    protected virtual void Release()
    {
       if (!gameObject.activeSelf)
        return;

        if (rangedWeapon == null)
        {
            Debug.LogError("RangedWeapon reference is missing! Did you forget to call Configure()?");
            DestroyBullet(); 
            return;
        }

        rangedWeapon.ReleaseBullet(this);
    }

    public void Reload()
    {
        target = null;

        rb.linearVelocity = Vector2.zero;
        col.enabled = true;

    }

}
