using UnityEngine;

public class FireballProjectile : MonoBehaviour
{
    private float lifeTime = 5f;
    private int damage;
    [SerializeField] protected LayerMask enemyMask;
    protected Enemy target;

    public void Initialize(int damageValue, float activeTime)
    {
        damage = damageValue;
        lifeTime = activeTime;
        Destroy(gameObject, lifeTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (target != null)
            return;

        if (IsInLayerMask(other.gameObject.layer, enemyMask))
        {
            target = other.GetComponent<Enemy>();

            if (target != null)
            {
                target.TakeDamage(damage, false); // Assuming no critical hit for fireball
            }
        }

        Destroy(gameObject);
    }

    protected bool IsInLayerMask(int layer, LayerMask layerMask) => (layerMask.value & (1 << layer)) != 0;
}