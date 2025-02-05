using UnityEngine;

[RequireComponent(typeof(EnemyMovement))]
[RequireComponent(typeof(RangedEnemyAttack))]
public class ShellshockBoss : Boss
{
    [Header("STAGE 2")]
    [SerializeField] private float stageTwoCooldown = 1.5f; 
    [SerializeField] private float stageTwoHealthThreshold = 0.5f;

    private RangedEnemyAttack rangedAttack;
    private bool isEnraged;

    protected override void InitializeBoss()
    {
        base.InitializeBoss();
        rangedAttack = GetComponent<RangedEnemyAttack>();
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        attackTimer = attackCooldown;
        rangedAttack.StorePlayer(character); // Ensure player targeting
    }

    protected override void Update()
    {
        base.Update();

        if (!hasSpawned) return;

        attackTimer -= Time.deltaTime;

        // **Check if Stage 2 should activate**
        if (!isEnraged && (float)health / maxHealth <= stageTwoHealthThreshold)
        {
            AdvanceToNextStage();
        }

        // **Check if boss should attack**
        if (attackTimer <= 0)
        {
            ExecuteStage();
            attackTimer = isEnraged ? stageTwoCooldown : attackCooldown;
        }

        // **Follow the player if not attacking**
        movement.FollowCurrentTarget();
    }

    protected override void ExecuteStage()
    {
        if (isEnraged)
        {
            // **Stage 2: Fires a burst of 3 shots**
            for (int i = 0; i < 3; i++)
            {
                Invoke(nameof(FireBullet), i * 0.2f); // Small delay between shots
            }
        }
        else
        {
            // **Stage 1: Fires a single shot**
            FireBullet();
        }
    }

    private void FireBullet()
    {
        rangedAttack.AutoAim(); // Uses the RangedEnemyAttack system to fire
    }

    protected override void AdvanceToNextStage()
    {
        isEnraged = true;
        Debug.Log("Ranged Boss is now in Stage 2!");
        transform.localScale *= 1.2f; // Slightly grows
    }
}
