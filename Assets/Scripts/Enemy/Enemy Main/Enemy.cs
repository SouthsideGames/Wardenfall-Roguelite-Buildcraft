using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(EnemyStatus))]
[RequireComponent(typeof(EnemyModifierHandler))]
[RequireComponent(typeof(EnemySpawnHandler))]
[RequireComponent(typeof(EnemyMissionTracker))]
[RequireComponent(typeof(EnemyTargetController))]
[RequireComponent(typeof(EnemyEvolutionHandler))]
[RequireComponent(typeof(EnemyMovement))]
public abstract class Enemy : MonoBehaviour, IDamageable, IEnemyBehavior
{
    [Header("DATA:")]
    public EnemyDataSO enemyData;

    [Header("ACTIONS:")]
    public static Action<int, Vector2, bool> OnDamageTaken;
    public static Action<Vector2> OnDeath;
    public static Action<Vector2> OnBossDeath;
    public static Action OnEnemyKilled;
    public Action OnSpawnCompleted;
    public Action<int> OnDealDamage;
    public Action OnHealthChanged;

    [Header("ELEMENTS:")]
    protected CharacterManager character;
    [SerializeField] protected SpriteRenderer _spriteRenderer;
    [SerializeField] protected SpriteRenderer spawnIndicator;
    [SerializeField] private Collider2D _collider;
    protected bool hasSpawned = false;

    [Header("ATTACK:")]
    [HideInInspector] public int contactDamage;
    [HideInInspector] public float playerDetectionRadius;
    protected float attackTimer;
    protected bool attacksEnabled = true;

    [Header("EFFECTS:")]
    [SerializeField] protected ParticleSystem deathParticles;

    [Header("DEBUG:")]
    [SerializeField] private bool showGizmos;

    [HideInInspector] public int maxHealth;
    [HideInInspector] public int health;
    [HideInInspector] public bool isInvincible = false;
    public int CurrentHealth => health;
    public int MaxHealth => maxHealth;
    public bool IsAlive => health > 0;

    public EnemyStatus status { get; private set; }
    public EnemyModifierHandler modifierHandler { get; private set; }
    public EnemyMovement movement { get; private set; }
    public CharacterManager Character => character;
    public Collider2D Collider => _collider;
    public SpriteRenderer SpriteRenderer => _spriteRenderer;
    public SpriteRenderer SpawnIndicator => spawnIndicator;
    public Transform PlayerTransform { get; set; }

    // Cached references to modular components
    private EnemySpawnHandler spawnHandler;
    private EnemyMissionTracker missionTracker;
    private EnemyTargetController targetController;
    private EnemyEvolutionHandler evolutionHandler;

    private void Awake()
    {
        if (enemyData != null)
            Initialize(enemyData);
    }

    protected virtual void Start()
    {

        health = maxHealth;

        character = FindFirstObjectByType<CharacterManager>();
        movement = GetComponent<EnemyMovement>();
        status = GetComponent<EnemyStatus>();
        modifierHandler = GetComponent<EnemyModifierHandler>();
        spawnHandler = GetComponent<EnemySpawnHandler>();
        missionTracker = GetComponent<EnemyMissionTracker>();
        targetController = GetComponent<EnemyTargetController>();
        evolutionHandler = GetComponent<EnemyEvolutionHandler>();

        if (character == null)
        {
            Debug.LogWarning("No player found");
            Destroy(gameObject);
            return;
        }

        PlayerTransform = character.transform;

        spawnHandler.BeginSpawn();
    }

    public virtual void Initialize(EnemyDataSO data)
    {
        maxHealth = data.maxHealth;
        health = maxHealth;
        contactDamage = data.contactDamage;
        playerDetectionRadius = data.detectionRadius;

        spawnHandler.SetSpawnValues(data.spawnSize, data.spawnTime, data.numberOfLoops);

    }

    protected virtual void Update()
    {
        if (!attacksEnabled) return;
        ChangeDirections();
    }

    protected virtual void ChangeDirections()
    {
        if (PlayerTransform != null)
        {
            _spriteRenderer.flipX = PlayerTransform.position.x > transform.position.x;
        }
    }

    protected virtual void Attack()
    {
        if (!attacksEnabled) return;
        attackTimer = 0;

        if (modifierHandler.CanCrit())
        {
            float critChance = (UnityEngine.Random.Range(0, 5) / 100f) * modifierHandler.GetCritChanceModifier();
            if (critChance >= CharacterManager.Instance.stats.GetStatValue(Stat.CritResist))
            {
                int critDamage = Mathf.FloorToInt(contactDamage * 2 * modifierHandler.GetDamageMultiplier());
                character.TakeDamage(critDamage);
                return;
            }
        }

        int scaledDamage = Mathf.FloorToInt(contactDamage * modifierHandler.GetDamageMultiplier());
        character.TakeDamage(scaledDamage);
    }

    public virtual void TakeDamage(int damage, bool isCritical)
    {
        if (isInvincible || !hasSpawned || this == null || gameObject == null) return;

        int realDamage = Mathf.Min(damage, health);
        health -= realDamage;

        OnDamageTaken?.Invoke(damage, transform.position, isCritical);

        if (CurrentHealth <= 0)
            DieByPlayer();
    }

    public void TakeDamage(int damage)
    {
        LeanTween.scale(gameObject, Vector3.one * 1.2f, 0.1f).setEasePunch();
        LeanTween.color(gameObject, Color.red, 0.1f).setLoopPingPong(1);
        TakeDamage(damage, false);
    }

    public void ApplyLifeDrain(int damage, float duration, float interval)
    {
        StatusEffect effect = new(StatusEffectType.Drain, duration, damage, interval);
        status.ApplyEffect(effect);
    }

    public virtual void DieByPlayer()
    {
        if (this == null || gameObject == null) return;

        StatisticsManager.Instance?.TotalEnemyKillsHandler();

        OnDeath?.Invoke(transform.position);
        OnEnemyKilled?.Invoke();

        if (TraitManager.Instance != null && TraitManager.Instance.HasTrait("T-007"))
        {
            GameObject ghostPrefab = Resources.Load<GameObject>("Prefabs/Enemy/Enemy Ghost");
            if (ghostPrefab != null)
            {
                GameObject ghost = Instantiate(ghostPrefab, transform.position, Quaternion.identity);
                GhostEnemy ghostComp = ghost.GetComponent<GhostEnemy>();
                int stacks = TraitManager.Instance.GetStackCount("T-007");

                if (ghostComp != null && _spriteRenderer != null)
                    ghostComp.InitializeFrom(_spriteRenderer, stacks);
            }
        }

        missionTracker.ReportKill();
        Die();
    }

    public virtual void Die()
    {
        if (deathParticles != null)
        {
            deathParticles.transform.SetParent(null);
            deathParticles.Play();
        }

        Destroy(gameObject);
    }

    public void DieAfterWave()
    {
        if (deathParticles != null)
        {
            deathParticles.transform.SetParent(null);
            deathParticles.Play();
        }

        Destroy(gameObject);
    }

    public void DisableAttacks() => attacksEnabled = false;
    public void EnableAttacks() => attacksEnabled = true;

    public void SetTargetToOtherEnemies() => targetController.SetTargetToOtherEnemies();
    public void ResetTarget() => targetController.SetTargetToPlayer();
    public Enemy FindClosestWoundedAlly(float radius) => targetController.FindClosestWoundedAlly(radius);

    public void Heal(int healAmount) => health += healAmount;
    public Vector2 GetCenter() => (Vector2)transform.position + _collider.offset;
    public void MoveTo(Transform target) => movement?.SetTarget(target);
    public void AttackTarget() => Attack();
    protected bool CanAttack() => _spriteRenderer.enabled;

    public virtual void ApplyEffect(StatusEffect effect)
    {
        if (status != null)
            status.ApplyEffect(effect);
    }

    public virtual void UpdateBehavior()
    {
        if (!hasSpawned || !attacksEnabled) return;
        ChangeDirections();
    }

    public virtual void OnHit()
    {
        if (status != null)
        {
            isInvincible = true;
            StartCoroutine(ResetInvincibility());
        }
    }

    private IEnumerator ResetInvincibility()
    {
        yield return new WaitForSeconds(0.1f);
        isInvincible = false;
    }

    public void TryEvolve()
    {
        if (evolutionHandler.CanEvolve())
            evolutionHandler.Evolve();
    }

    public void MarkAsSpawned() => hasSpawned = true;

    private void OnDrawGizmos()
    {
        if (!showGizmos) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, playerDetectionRadius);
    }
}
