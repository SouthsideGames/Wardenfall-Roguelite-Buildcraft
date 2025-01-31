using UnityEngine;
using MoreMountains.Feedbacks;
using System.Collections;

public class BramblethornBoss : Enemy
{
    private EnemyMovement enemyMovement;
    private EnemyStatus enemyStatus;
    
    [Header("Movement Settings")]
    [SerializeField] private float normalSpeed = 1.5f;
    [SerializeField] private float chargeSpeedMultiplier = 3f;
    [SerializeField] private float regrowthSlowdown = 0.8f;

    [Header("Attack Settings")]
    [SerializeField] private BramblethornAttackSettings attackSettings;

    private bool isAttacking = false;
    private bool isCharging = false;
    private bool isRegrowthActive = false;

    [Header("FEEL Feedbacks")]
    [SerializeField] private MMFeedbacks moveFeedback;
    [SerializeField] private MMFeedbacks chargeFeedback;
    [SerializeField] private MMFeedbacks stopFeedback;
    [SerializeField] private MMFeedbacks rootSlamFeedback;
    [SerializeField] private MMFeedbacks thornBarrageFeedback;
    [SerializeField] private MMFeedbacks regrowthFeedback;

    protected override void Start()
    {
        base.Start();
        enemyMovement = GetComponent<EnemyMovement>();
        enemyStatus = GetComponent<EnemyStatus>();

        if (enemyMovement == null) Debug.LogError("EnemyMovement missing on " + gameObject.name);
        if (enemyStatus == null) Debug.LogError("EnemyStatus missing on " + gameObject.name);

        enemyMovement.moveSpeed = normalSpeed;
        StartCoroutine(BossAI());
    }

    private IEnumerator BossAI()
    {
        while (true)
        {
            if (!isAttacking && !isCharging && !enemyStatus.IsStunned)
                enemyMovement.FollowCurrentTarget();

            yield return new WaitForSeconds(0.1f);
        }
    }

    private IEnumerator PerformAttack(System.Func<IEnumerator> attackCoroutine, float cooldown)
    {
        if (isAttacking) yield break;
        isAttacking = true;

        yield return StartCoroutine(attackCoroutine());

        isAttacking = false;
        yield return new WaitForSeconds(cooldown);
    }

    private IEnumerator RootSlam()
    {
         Debug.Log("Bramblethorn: Root Slam!");
        PlayAnimation("RootSlam");

        // Disable movement during the attack
        enemyMovement.DisableMovement(attackSettings.attackPauseTime);
        rootSlamFeedback?.PlayFeedbacks(); // FEEL Effect

        yield return new WaitForSeconds(1f); // Wind-up time

        // ✅ Damage enemies in an area
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, attackSettings.rootSlamRadius);
        foreach (Collider2D enemy in hitEnemies)
        {
            if (enemy.CompareTag("Player"))
                enemy.GetComponent<CharacterHealth>().TakeDamage(attackSettings.slamDamage);
        }

        // ✅ Spawn spikes in a radial pattern
        SpawnRootSlamSpikes();

        yield return new WaitForSeconds(attackSettings.attackPauseTime);
    }

    private IEnumerator ThornBarrage()
    {
        Debug.Log("Bramblethorn: Thorn Barrage!");
        PlayAnimation("ThornBarrage");
        enemyMovement.DisableMovement(attackSettings.attackPauseTime);
        thornBarrageFeedback?.PlayFeedbacks();

        for (int i = 0; i < attackSettings.thornProjectileCount; i++)
        {
            GameObject thorn = Instantiate(attackSettings.thornProjectilePrefab, attackSettings.thornSpawnPoint.position, Quaternion.identity);
            Rigidbody2D rb = thorn.GetComponent<Rigidbody2D>();
            rb.linearVelocity = new Vector2(Random.Range(-1f, 1f), 1).normalized * attackSettings.thornProjectileSpeed;
            yield return new WaitForSeconds(0.2f);
        }
    }

    private IEnumerator BristleCharge()
    {
        if (isCharging) yield break;
        isCharging = true;

        Debug.Log("Bramblethorn: Bristle Charge!");
        PlayAnimation("ChargeStart");

        yield return new WaitForSeconds(1f);

        enemyMovement.moveSpeed *= chargeSpeedMultiplier;
        chargeFeedback?.PlayFeedbacks();
        PlayAnimation("Charging");

        yield return new WaitForSeconds(2f);

        enemyMovement.moveSpeed = normalSpeed;
        PlayAnimation("ChargeEnd");

        yield return new WaitForSeconds(attackSettings.chargeCooldown);
        isCharging = false;
    }

    private void ActivateRegrowthArmor()
    {
        if (isRegrowthActive) return;
        isRegrowthActive = true;

        Debug.Log("Bramblethorn: Regrowth Armor Activated!");
        PlayAnimation("RegrowthArmor");
        regrowthFeedback?.PlayFeedbacks();

        enemyMovement.moveSpeed *= regrowthSlowdown;
    }

    protected override void Update()
    {
        base.Update();
        if (!isRegrowthActive && health <= maxHealth * attackSettings.regrowthThreshold)
            ActivateRegrowthArmor();
    }

    private void SpawnRootSlamSpikes()
    {
        float angleStep = 360f / attackSettings.numberOfSpikes;

        for (int i = 0; i < attackSettings.numberOfSpikes; i++)
        {
            float angle = i * angleStep;
            Vector2 direction = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));

            GameObject spike = Instantiate(attackSettings.spikePrefab, transform.position, Quaternion.identity);
            Rigidbody2D rb = spike.GetComponent<Rigidbody2D>();

            if (rb != null)
            {
                rb.linearVelocity = direction * attackSettings.spikeSpeed;
            }

            Destroy(spike, attackSettings.spikeLifetime); 
        }
    }

    private void PlayAnimation(string animationName)
    {
        if (anim)
            anim.Play(animationName);
    }
}

