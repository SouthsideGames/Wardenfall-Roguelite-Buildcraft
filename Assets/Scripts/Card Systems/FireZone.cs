using UnityEngine;

public class FireZone : MonoBehaviour
{
    [SerializeField] private float duration;
    private int damage;
    private LayerMask enemyMask;

    public void Initialize(int _damage, LayerMask mask)
    {
        damage = _damage;
        enemyMask = mask;

        Destroy(gameObject, duration);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (((1 << collision.gameObject.layer) & enemyMask) != 0)
        {
            Enemy enemy = collision.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage, false);
            }
        }
    }
}
