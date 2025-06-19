using UnityEngine;

public class MinionAttack : MonoBehaviour
{
    [Header("ATTACK SETTINGS:")]
    [SerializeField] private float attackRange;
    [SerializeField] private float attackCooldown;

    private Enemy targetEnemy;
    private MinionManager minionManager;
    private float attackTimer;

    private void Awake()
    {
        minionManager = GetComponent<MinionManager>();
    }

    private void Update()
    {
        if (targetEnemy != null && Vector2.Distance(transform.position, targetEnemy.transform.position) <= attackRange)
        {
            PerformAttack();
        }

        attackTimer -= Time.deltaTime;
    }

    public void SetTargetEnemy(Enemy enemy)
    {
        targetEnemy = enemy;
    }

    private void PerformAttack()
    {
        if (attackTimer <= 0)
        {
            targetEnemy.TakeDamage(minionManager.GetDamage());
            attackTimer = attackCooldown;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
