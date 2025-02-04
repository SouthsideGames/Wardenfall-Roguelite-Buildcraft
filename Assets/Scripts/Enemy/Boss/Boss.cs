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
    }

    protected override void Update()
    {
        base.Update();  
        
        ManageStates();
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
        ExecuteAttack();
    }

    protected virtual void ExecuteAttack()
    {

    }

    private void ManageAttackingState()
    {
        if (bossState == BossState.Attacking)
        {
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

    protected virtual void InitializeBoss() { }

}
