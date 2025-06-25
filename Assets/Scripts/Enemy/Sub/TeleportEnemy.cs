using UnityEngine;

// Optional wrapper for enemies that want to react to teleports
public class TeleportEnemy : RangedEnemy
{
    private EnemyAnimator enemyAnimator;

    protected override void Start()
    {
        base.Start();
        enemyAnimator = GetComponent<EnemyAnimator>();
        enemyAnimator?.PlayIdlePulseAnimation(); // Visual idle animation
    }

    protected override void Update()
    {
        base.Update();

        if (!CanAttack())
            return;

        AttackLogic();

        // Optional: Flip sprite based on player position
        if (character != null)
        {
            transform.localScale = character.transform.position.x > transform.position.x
                ? Vector3.one
                : Vector3.one.With(x: -1);
        }

        // OPTIONAL: Do something immediately after a teleport
        if (movement.teleportMovement && movement.TeleportJustHappened())
        {
            OnTeleportEffect();
        }
    }

    private void AttackLogic()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, character.transform.position);

        if (distanceToPlayer <= playerDetectionRadius)
            TryAttack();
        else
            movement.FollowCurrentTarget();
    }

    private void OnTeleportEffect() => enemyAnimator?.PlayTeleportFlash();

}
