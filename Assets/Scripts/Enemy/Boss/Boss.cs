using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Random = UnityEngine.Random;

public class Boss : Enemy
{
    [Header("ADD. ELEMENTS:")]
    [SerializeField] private Slider healthBar;
    [SerializeField] private TextMeshProUGUI healthText;

    protected BossState bossState;

    [Header("IDLE STATE:")]
    [SerializeField] private float maxIdleDuration = 1f;
    private float idleDuration;
    private float idleTimer;

    [Header("MOVING STATE:")]
    protected Vector2 targetPosition;

    [Header("BOSS STAGE SYSTEM")]
    [SerializeField] protected float attackCooldown = 3f; // Time between attacks
    [SerializeField] private int maxStages = 3; // Number of phases
    [SerializeField] protected int currentStage = 1;

    private void Awake()
    {
        bossState = BossState.None;
        healthBar.gameObject.SetActive(false);

        OnSpawnCompleted += SpawnCompletedCallback;
        OnDamageTaken += DamageTakenCallback;
    }

    private void OnDestroy()
    {
        OnSpawnCompleted -= SpawnCompletedCallback;
        OnDamageTaken -= DamageTakenCallback;
    }

    protected override void Start()
    {
        base.Start();
        InitializeBoss();
        attackTimer = attackCooldown;
    }

    protected override void Update()
    {
        base.Update();
        UpdateHealthUI();
        ManageStates();

        // Decrease the attack cooldown timer
        attackTimer -= Time.deltaTime;

        if (attackTimer <= 0 && bossState == BossState.Attacking)
        {
            ExecuteStage();
            attackTimer = attackCooldown; // Reset attack cooldown
        }
    }

    protected virtual void ManageStates()
    {
        switch (bossState)
        {
            case BossState.Idle:
                ManageIdleState();
                break;
            case BossState.Moving:
                ManageMovingState();
                break;
            case BossState.Attacking:
                ManageAttackingState();
                break;
            default:
                break;
        }
    }

    protected void SetIdleState()
    {
        bossState = BossState.Idle;
        idleDuration = Random.Range(1f, maxIdleDuration);
        anim.Play("Idle");
    }

    protected virtual void ManageIdleState()
    {
        idleTimer += Time.deltaTime;
        if (idleTimer >= idleDuration)
        {
            idleTimer = 0;
            StartMovingState();
        }
    }

    protected virtual void StartMovingState()
    {
        bossState = BossState.Moving;
        targetPosition = GetRandomPosition();
        anim.Play("Move");
    }

    protected virtual void ManageMovingState()
    {
        transform.position = Vector2.MoveTowards(transform.position, targetPosition, movement.moveSpeed * Time.deltaTime);

        if (Vector2.Distance(transform.position, targetPosition) < 0.1f)
            StartAttackingState();
    }

    protected void StartAttackingState()
    {
        bossState = BossState.Attacking;
        anim.Play("Attack");
        ExecuteStage(); // Calls a stage attack based on the current phase
    }

    private void ManageAttackingState()
    {
        if (bossState == BossState.Attacking)
        {
            Invoke("SetIdleState", 1.5f);
        }
    }

    protected virtual void ExecuteStage()
    {
        int stageToExecute = Random.Range(1, currentStage + 1); // Randomly select a stage up to the current one

        switch (stageToExecute)
        {
            case 1:
                ExecuteStageOne();
                break;
            case 2:
                ExecuteStageTwo();
                break;
            case 3:
                ExecuteStageThree();
                break;
            default:
                Debug.LogWarning("Invalid Stage Selected");
                break;
        }
    }

    protected virtual void ExecuteStageOne() { }
    protected virtual void ExecuteStageTwo() { }
    protected virtual void ExecuteStageThree() { }

    // Call this to progress to the next stage
    protected virtual void AdvanceToNextStage()
    {
        if (currentStage < maxStages)
        {
            currentStage++;
            Debug.Log($"Boss advanced to Stage {currentStage}");

            SetIdleState();
        }
    }

    protected void UpdateHealthUI()
    {
        healthBar.value = (float)health / maxHealth;
        healthText.text = $"{health} / {maxHealth}";
    }

    private void SpawnCompletedCallback()
    {
        UpdateHealthUI();
        healthBar.gameObject.SetActive(true);
        SetIdleState();
    }

    private Vector2 GetRandomPosition()
    {
        Vector2 targetPosition = Vector2.zero;
        targetPosition.x = Random.Range(-Constants.arenaSize.x / 3, Constants.arenaSize.x / 3);
        targetPosition.y = Random.Range(-Constants.arenaSize.y / 3, Constants.arenaSize.y / 3);
        return targetPosition;
    }

    protected override void Die()
    {
        OnBossDeath?.Invoke(transform.position);
        DieAfterWave();
    }

    private void DamageTakenCallback(int _damage, Vector2 _position, bool _isCriticalHit) => UpdateHealthUI();
    protected virtual void InitializeBoss() { }

}
