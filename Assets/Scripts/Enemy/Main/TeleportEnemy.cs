using UnityEngine;

//TODO: Add Teleport Feedback
public class TeleportEnemy : RangedEnemy
{
     [Header("TELEPORT SPECIFICS:")]
    [SerializeField] private float teleportCooldown = 5f;
    [SerializeField] private float teleportDistance = 5f;
    private float nextTeleportTime;

    protected override void Start()
    {
        base.Start();
        nextTeleportTime = Time.time;
    }

    protected override void Update()
    {
        base.Update();  
        
        if (!CanAttack())
            return;

        AttackLogic();

        if (Time.time >= nextTeleportTime)
        {
            Teleport();
            nextTeleportTime = Time.time + teleportCooldown;
        }

        transform.localScale = character.transform.position.x > transform.position.x ? Vector3.one : Vector3.one.With(x: -1);
    }

    private void Teleport()
    {
        Vector2 randomDirection = Random.insideUnitCircle.normalized;
        Vector2 newPosition = (Vector2)transform.position + randomDirection * teleportDistance;
        transform.position = newPosition;

        // Optionally add teleportation effects here (particles, sound, etc.)
    }

    private void AttackLogic()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, character.transform.position);

        if (distanceToPlayer <= playerDetectionRadius)
            TryAttack();
        else
            movement.FollowCurrentTarget();
    }
}
