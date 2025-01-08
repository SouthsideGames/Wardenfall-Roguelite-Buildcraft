using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Random = UnityEngine.Random;

[RequireComponent(typeof(RangedEnemyAttack))]
public class Boss : Enemy
{
    [Header("ADD. ELEMENTS:")]
    [SerializeField] private Slider healthBar;
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] protected Animator anim;

    protected BossState bossState;

    [Header("IDLE STATE:")]
    [SerializeField] private float maxIdleDuration = 1f;
    private float idleDuration;
    private float idleTimer;

    [Header("MOVING STATE:")]
    [SerializeField] protected float moveSpeed;
    protected Vector2 targetPosition;

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
        InitializeBoss(); // For boss-specific setup
    }

    protected override void Update()
    {
        ManageStates();
    }

    private void ManageStates()
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

    private void SetIdleState()
    {
        bossState = BossState.Idle;

        idleDuration = Random.Range(1f, maxIdleDuration);

        anim.Play("Idle");
    }

    private void ManageIdleState()
    {
        idleTimer += Time.deltaTime;

        if (idleTimer >= idleDuration)
        {
            idleTimer = 0;
            StartMovingState();
        }
    }

    private void StartMovingState()
    {
        bossState = BossState.Moving;

        targetPosition = GetRandomPosition();
        anim.Play("Move");
    }

    private void ManageMovingState()
    {
        transform.position = Vector2.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

        if (Vector2.Distance(transform.position, targetPosition) < 0.1f)
            StartAttackingState();
    }

    private void StartAttackingState()
    {
        bossState = BossState.Attacking;
        anim.Play("Attack");
        ExecuteAttack(); // Boss-specific attack logic
    }

    // Override this in child classes
    protected virtual void ExecuteAttack()
    {

    }

    private void ManageAttackingState()
    {
        if (bossState == BossState.Attacking)
        {
            // Default Attack Delay - Override in derived classes
            Invoke("SetIdleState", 1.5f);
        }
    }

    private void UpdateHealthUI()
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

    // Boss-specific initialization
    protected virtual void InitializeBoss() { }

}
