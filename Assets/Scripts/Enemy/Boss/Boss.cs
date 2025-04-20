using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Random = UnityEngine.Random;
using SouthsideGames.DailyMissions;

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
    [SerializeField] protected float attackCooldown = 3f;
    [SerializeField] private int numberOfStages = 3;
    protected int stageToExecute; 

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

        attackTimer -= Time.deltaTime;

        if (attackTimer <= 0 && bossState == BossState.Attacking)
        {
            ExecuteStage();
            attackTimer = attackCooldown;
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
        stageToExecute = Random.Range(1, numberOfStages + 1); // Randomly select a stage from 1 to `numberOfStages`

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
        return new Vector2(
            Random.Range(-Constants.arenaSize.x / 3, Constants.arenaSize.x / 3),
            Random.Range(-Constants.arenaSize.y / 3, Constants.arenaSize.y / 3)
        );
    }

    public override void Die()
    {
        MissionManager.Increment(MissionType.bossHunter, 1);
        OnBossDeath?.Invoke(transform.position);
        DieAfterWave();
    }

    private void DamageTakenCallback(int _damage, Vector2 _position, bool _isCriticalHit) => UpdateHealthUI();
    protected virtual void InitializeBoss() { }
}
