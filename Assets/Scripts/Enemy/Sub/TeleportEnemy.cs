using UnityEngine;

// Optional wrapper for enemies that want to react to teleports
public class TeleportEnemy : RangedEnemy
{

    protected override void Start()
    {
        base.Start();
        enemyAnimator?.PlayIdlePulseAnimation();
    }

    protected override void Update()
    {
        base.Update();

        if (!CanAttack())
            return;

        AttackLogic();

        if (character != null)
        {
            transform.localScale = character.transform.position.x > transform.position.x
                ? Vector3.one
                : Vector3.one.With(x: -1);
        }

        if (movement.teleportMovement && movement.TeleportJustHappened())
            OnTeleportEffect();
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
