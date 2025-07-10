using UnityEngine;

public class BulletBase : MonoBehaviour
{
    [Header("BULLET SETTINGS")]
    [SerializeField] protected float moveSpeed = 10f;
    [SerializeField] protected float lifetime = 3f;
    [SerializeField] protected LayerMask enemyMask;

    public int damage { get; private set; }
    protected bool isCriticalHit;
    protected Rigidbody2D rb;
    private Collider2D col;
    public RangedWeapon rangedWeapon { get; private set; }
    protected Enemy target;
    private bool destroyOnHit = true;

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

                foreach (var mod in GetComponents<IBulletModifier>())
                    mod.Apply(this, target);

                if (destroyOnHit)
                    Release();

                destroyOnHit = true;
                Release();
            }
            else if (collision.TryGetComponent<EvoCrystal>(out var crystal))
            {
                CancelInvoke();
                crystal.TakeDamage(damage);

                foreach (var mod in GetComponents<IBulletModifier>())
                    mod.Apply(this, null); // still apply modifiers if needed

                if (destroyOnHit)
                    Release();

                destroyOnHit = true;
                Release();
            }
        }
    }


    protected virtual void ApplyDamage(Enemy enemy)
    {
        int finalDamage = damage;
        
        var cm = CharacterManager.Instance;
        if (cm.cards.HasCard("S-023"))
        {
            EnemyStatus status = enemy.GetComponent<EnemyStatus>();
            if (status != null && status.HasAnyEffect())
                finalDamage = Mathf.RoundToInt(damage * 1.1f);
        }

        enemy.TakeDamage(finalDamage, isCriticalHit);
    }

    protected virtual void ApplyOnHitEffects(Enemy enemy)
    {
        CharacterManager cm = CharacterManager.Instance;
        EnemyStatus status = enemy.GetComponent<EnemyStatus>();

        if (status == null) return;

        if (cm.cards.HasCard("S-024"))
        {
            float chance = 0.15f;
            if (Random.value < chance)
            {
                StatusEffect burn = new StatusEffect(StatusEffectType.Burn, 3f, 5f, 1f);
                status.ApplyEffect(burn);
            }
        }

        if (cm.cards.HasCard("S-081"))
        {
            StatusEffect poison = new StatusEffect(StatusEffectType.Poison, 2f, 5f, 1f, 3);
            status.ApplyEffect(poison);
        }

        if (cm.cards.HasCard("S-026"))
        {
            float chance = 0.35f;
            if (Random.value < chance)
            {
                StatusEffect fear = new StatusEffect(StatusEffectType.Fear, 4f, 0f, 1f);
                status.ApplyEffect(fear);
            }
        }
    }

    protected virtual void DestroyBullet() => gameObject.SetActive(false);
    protected bool IsInLayerMask(int layer, LayerMask layerMask) => (layerMask.value & (1 << layer)) != 0;
    public void CancelDestroyOnHit() => destroyOnHit = false;

    public virtual void Release()
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
