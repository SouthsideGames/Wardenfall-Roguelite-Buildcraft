using UnityEngine;

public class SplitterEnemy : Enemy
{
    [Header("SPLITTER SETTINGS")]
    [SerializeField] private int maxSplits = 3;
    [SerializeField] private GameObject smallerVersionPrefab;
    [SerializeField] private GameObject splitEffectPrefab;

    private int splitCount = 0;
    private EnemyAnimator enemyAnimator;

    protected override void Start()
    {
        base.Start();
        enemyAnimator = GetComponent<EnemyAnimator>();
    }

    public override void Die()
    {
        if (splitCount < maxSplits)
        {
            Split();
        }
        else
        {
            base.Die();
        }
    }

    private void Split()
    {
        splitCount++;

        for (int i = 0; i < 2; i++)
        {
            Instantiate(smallerVersionPrefab, transform.position, Quaternion.identity);
        }

        if (splitEffectPrefab != null)
        {
            GameObject effect = Instantiate(splitEffectPrefab, transform.position, Quaternion.identity);
            Destroy(effect, 2f);
        }

        Destroy(gameObject);
    }

    public void SetSplitCount(int value) => splitCount = value;

    protected override void Update()
    {
        base.Update();

        if (!CanAttack()) return;

        attackTimer += Time.deltaTime;

        if (attackTimer >= 1f)
            TryAttack();
        else
            enemyAnimator?.PlayMoveAnimation(); // Only play if not attacking

        movement.FollowCurrentTargetUntilRange(1.5f); // or whatever stop distance works visually

    }

    private void TryAttack()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, character.transform.position);
        if (distanceToPlayer <= playerDetectionRadius)
        {
            enemyAnimator?.PlayAttackAnimation();
            Attack();
        }
    }
}
