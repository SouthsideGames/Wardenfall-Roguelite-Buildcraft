using System;
using System.Collections;
using System.Collections.Generic;
using SouthsideGames.DailyMissions;
using UnityEngine;

[RequireComponent(typeof(EnemyStatus))]
[RequireComponent(typeof(EnemyModifierHandler))]
public abstract class Enemy : MonoBehaviour, IDamageable, IEnemyBehavior
{
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
    [HideInInspector] public EnemyMovement movement;
    [SerializeField] protected SpriteRenderer _spriteRenderer;
    [SerializeField] protected SpriteRenderer spawnIndicator;
    [SerializeField] private Collider2D _collider;
    protected bool hasSpawned = false;

    [Header("ATTACK:")]
    [HideInInspector] public int contactDamage;
    public float playerDetectionRadius;
    protected float attackTimer;
    private bool attacksEnabled = true;

    [Header("HEALTH:")]
    [HideInInspector] public int maxHealth;
    [HideInInspector] public int health;
    [HideInInspector] public bool isInvincible = false;

    [Header("SPAWN VALUES:")]
    [SerializeField] private float spawnSize = 1.2f;
    [SerializeField] private float spawnTime = 0.3f;
    [SerializeField] private int numberOfLoops = 4;

    [Header("EFFECTS:")]
    [SerializeField] protected ParticleSystem deathParticles;

    [Header("DEBUG:")]
    [SerializeField] private bool showGizmos;

    [Header("EVOLUTION")]
    [SerializeField] private bool isEvolvable = false;
    private bool hasEvolved = false;

    public EnemyStatus status { get; private set; }
    public EnemyModifierHandler modifierHandler { get; private set; }
    protected Transform playerTransform;
    public int CurrentHealth => health;
    public int MaxHealth => maxHealth;
    public bool IsAlive => health > 0;

    private static float multiKillTimeWindow = 0.5f;
    private static float lastKillTime;
    private static int simultaneousKills;

    protected virtual void Start()
    {
        health = maxHealth;

        movement = GetComponent<EnemyMovement>();
        character = FindFirstObjectByType<CharacterManager>();
        status = GetComponent<EnemyStatus>();
        modifierHandler = GetComponent<EnemyModifierHandler>();

        if (character == null)
        {
            Debug.LogWarning("No player found");
            Destroy(gameObject);
        }
        else
            playerTransform = character.transform;

        Spawn();
    }

    protected virtual void Update()
    {
        if (!attacksEnabled) return;
        ChangeDirections();
    }

    public virtual void Initialize(EnemyDataSO data)
    {
        maxHealth = data.maxHealth;
        health = maxHealth;
        contactDamage = data.contactDamage;
        playerDetectionRadius = data.detectionRadius;
    }

    protected virtual void ChangeDirections()
    {
        if (playerTransform != null)
        {
            _spriteRenderer.flipX = playerTransform.position.x > transform.position.x;
        }
    }

    protected virtual void Attack()
    {
        if (!attacksEnabled) return;

        attackTimer = 0;

        if (modifierHandler.CanCrit())
        {
            float enemyCritChance = (UnityEngine.Random.Range(0, 5) / 100f) * modifierHandler.GetCritChanceModifier();
            if (enemyCritChance >= CharacterManager.Instance.stats.GetStatValue(Stat.CritResist))
            {
                int critDamage = Mathf.FloorToInt(contactDamage * 2 * modifierHandler.GetDamageMultiplier());
                character.TakeDamage(critDamage);
                return;
            }
        }

        int scaledDamage = Mathf.FloorToInt(contactDamage * modifierHandler.GetDamageMultiplier());
        character.TakeDamage(scaledDamage);
    }

    public virtual void TakeDamage(int _damage, bool _isCriticalHit)
    {
        if (isInvincible || this == null || gameObject == null) return;

        int realDamage = Mathf.Min(_damage, health);
        health -= realDamage;

        OnDamageTaken?.Invoke(_damage, transform.position, _isCriticalHit);

        if (CurrentHealth <= 0)
        {
            DieByPlayer();
        }
    }

    public void TakeDamage(int damage)
    {
        LeanTween.scale(gameObject, Vector3.one * 1.2f, 0.1f).setEasePunch();
        LeanTween.color(gameObject, Color.red, 0.1f).setLoopPingPong(1);
        TakeDamage(damage, false);
    }

    public void ApplyLifeDrain(int damage, float duration, float interval)
    {
        StatusEffect drainEffect = new(StatusEffectType.Drain, duration, damage, interval);
        status.ApplyEffect(drainEffect);
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
                GhostEnemy ghostComponent = ghost.GetComponent<GhostEnemy>();

                int stacks = TraitManager.Instance.GetStackCount("T-007");

                if (ghostComponent != null && _spriteRenderer != null)
                    ghostComponent.InitializeFrom(_spriteRenderer, stacks);
            }
        }

        float timeSinceLastKill = Time.time - lastKillTime;
        if (timeSinceLastKill <= multiKillTimeWindow)
        {
            simultaneousKills++;
            if (simultaneousKills >= 3)
            {
                MissionManager.Increment(MissionType.multiKills, 1);
                MissionManager.Increment(MissionType.multiKills2, 1);
                MissionManager.Increment(MissionType.multiKills3, 1);
                MissionManager.Increment(MissionType.multiKills4, 1);
                WaveManager.Instance?.AdjustViewerScore(0.15f);
                simultaneousKills = 0;
            }
        }
        else
        {
            simultaneousKills = 1;
        }
        lastKillTime = Time.time;

        MissionIncrement();
        WaveManager.Instance?.ReportKill();
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

    #region SPAWN FUNCTIONS

    private void Spawn()
    {
        SetRenderersVisibility(false);
        movement.canMove = false;

        Vector3 targetScale = spawnIndicator.transform.localScale * spawnSize;
        LeanTween.scale(spawnIndicator.gameObject, targetScale, spawnTime)
            .setLoopPingPong(numberOfLoops)
            .setOnComplete(SpawnCompleted);
    }

    private void SetRenderersVisibility(bool visibility)
    {
        _spriteRenderer.enabled = visibility;
        spawnIndicator.enabled = !visibility;
    }

    protected virtual void SpawnCompleted()
    {
        SetRenderersVisibility(true);
        hasSpawned = true;
        _collider.enabled = true;

        movement?.StorePlayer(character);
        movement?.EnableMovement();
        OnSpawnCompleted?.Invoke();

        StartCoroutine(ApplyTraitsNextFrame());
    }

    #endregion

    #region TARGET & CONTROL

    public void DisableAttacks() => attacksEnabled = false;
    public void EnableAttacks() => attacksEnabled = true;

    public void SetTargetToOtherEnemies()
    {
        playerTransform = null;

        Enemy[] allEnemies = FindObjectsByType<Enemy>(FindObjectsSortMode.None);
        List<Enemy> validTargets = new();

        foreach (var enemy in allEnemies)
            if (enemy != this && enemy != null)
                validTargets.Add(enemy);

        if (validTargets.Count > 0)
        {
            int randomIndex = UnityEngine.Random.Range(0, validTargets.Count);
            Transform newTarget = validTargets[randomIndex].transform;
            movement?.SetTarget(newTarget);
        }
    }

    public void ResetTarget()
    {
        if (character != null)
        {
            playerTransform = character.transform;
            movement?.SetTarget(playerTransform);
        }
    }

    #endregion

    #region MISSION FUNCTIONS
    private void MissionIncrement()
    {
        MissionManager.Increment(MissionType.eliminate100Enemies, 1);
        MissionManager.Increment(MissionType.eliminate500Enemies, 1);
        MissionManager.Increment(MissionType.eliminate1000Enemies, 1);
        MissionManager.Increment(MissionType.eliminate2000Enemies, 1);
    }

    #endregion

    private IEnumerator ApplyTraitsNextFrame()
    {
        yield return null;
        modifierHandler?.ApplyTraits();
    }

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

    #region SET FUNCTIONS

    public void Heal(int _damage) => health += _damage;
    public Vector2 GetCenter() => (Vector2)transform.position + _collider.offset;
    public void MoveTo(Transform target) => movement?.SetTarget(target);
    public void AttackTarget() => Attack();
    protected bool CanAttack() => _spriteRenderer.enabled;

    public Enemy FindClosestWoundedAlly(float radius)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, radius);
        Enemy closest = null;
        float closestDist = Mathf.Infinity;

        foreach (Collider2D hit in hits)
        {
            if (hit.TryGetComponent<Enemy>(out Enemy ally) && ally != this)
            {
                if (ally.CurrentHealth < ally.MaxHealth)
                {
                    float dist = Vector2.Distance(transform.position, ally.transform.position);
                    if (dist < closestDist)
                    {
                        closest = ally;
                        closestDist = dist;
                    }
                }
            }
        }

        return closest;
    }

    #endregion

    #region DEBUGS & GIZMOS

    public void DisableAbilities() => Debug.Log("Enemy abilities disabled.");
    public void EnableAbilities() => Debug.Log("Enemy abilities enabled.");
    private void OnDrawGizmos()
    {
        if (!showGizmos) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, playerDetectionRadius);
    }

    #endregion

    #region EVOLUTION METHODS

    public bool CanEvolve() => isEvolvable && !hasEvolved && IsAlive;

    public void Evolve()
    {
        if (!CanEvolve()) return;

        hasEvolved = true;

        maxHealth = Mathf.FloorToInt(maxHealth * 1.5f);
        health = maxHealth;
        contactDamage = Mathf.FloorToInt(contactDamage * 1.5f);
        transform.localScale *= 1.25f;

        modifierHandler?.ApplyTraits();

        Debug.Log($"{gameObject.name} has evolved!");
    }

    #endregion
}
