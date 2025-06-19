using System.Collections;
using UnityEngine;

public class BerserkerEnemy : Enemy
{
    [Header("BERSERKER SPECIFICS:")]
    [SerializeField] private float enragedThreshold = 0.5f;
    [SerializeField] private float enragedAttackMultiplier = 2f;
    [SerializeField] private float enragedSpeedMultiplier = 1.5f;

    [SerializeField] private float attackRate;
    [SerializeField] private GameObject enrageIcon;

    private float attackDelay;
    private bool hasEnraged = false;
    private EnemyAnimator enemyAnimator;

    protected override void Start()
    {
        base.Start();
        attackDelay = 1f / attackRate;
        enemyAnimator = GetComponent<EnemyAnimator>();

        // Start idle animation
        enemyAnimator?.PlayIdlePulse();
    }

    protected override void Update()
    {
        base.Update();

        if (!CanAttack()) return;

        if (attackTimer >= attackDelay)
            TryAttack();
        else
            attackTimer += Time.deltaTime;

        if (!hasEnraged && health <= maxHealth * enragedThreshold)
            Enrage();

        movement.FollowCurrentTarget();

        if (hasEnraged)
            enemyAnimator?.PlayEnragePulse(); // Optional persistent rage glow
    }

    private void TryAttack()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, character.transform.position);

        if (distanceToPlayer <= playerDetectionRadius)
        {
            enemyAnimator?.PlayAttackBurst();
            Attack();
        }
    }

    private void Enrage()
    {
        contactDamage = Mathf.RoundToInt(contactDamage * enragedAttackMultiplier);
        movement.moveSpeed *= enragedSpeedMultiplier;
        hasEnraged = true;

        if (enrageIcon != null)
            enrageIcon.SetActive(true);

        enemyAnimator?.PlayEnrageBurst();
    }
}
