using System;
using System.Collections;
using System.Collections.Generic;
using SouthsideGames.DailyMissions;
using UnityEngine;

[RequireComponent(typeof(EnemyStatus))]
public abstract class Enemy : MonoBehaviour
{
    [Header("ACTIONS:")]
    public static Action<int, Vector2, bool> OnDamageTaken;
    public static Action<Vector2> OnDeath;
    public static Action<Vector2> OnBossDeath;
    public static Action OnEnemyKilled;
    protected Action OnSpawnCompleted;

    [Header("ELEMENTS:")]
    protected CharacterManager character;
    protected EnemyMovement movement;
    [SerializeField] protected Animator anim;
    [SerializeField] protected SpriteRenderer _spriteRenderer;
    [SerializeField] protected SpriteRenderer spawnIndicator;
    [SerializeField] private Collider2D _collider;
    protected bool hasSpawned = false;

    [Header("ATTACK:")]
    [SerializeField] protected int damage;
    [SerializeField] protected float playerDetectionRadius;
    protected float boxDetectionRadius;
    protected float attackTimer;
    private bool isCriticalHit;
    private bool attacksEnabled = true;

    [Header("HEALTH:")]
    public int maxHealth;
    [HideInInspector] public int health;
    [HideInInspector] public bool isInvincible = false;

    [Header("SPAWN VALUES:")]
    [SerializeField] private float spawnSize = 1.2f;
    [SerializeField] private float spawnTime = .3f;
    [SerializeField] private int numberOfLoops = 4;

    [Header("MODIFIER:")]
    [SerializeField] private bool canPerformCriticalHit;
    private float accuracyModifier = 1f;
    private float damageModifier = 1f;

    [Header("EFFECTS:")]
    [SerializeField] protected ParticleSystem deathParticles;

    [Header("DEBUG:")]
    [SerializeField] private bool showGizmos;

    private EnemyStatus status;
    protected Transform playerTransform;
    private SurvivorBox detectedBox;

    protected virtual void Start()
    {
        health = maxHealth;

        movement = GetComponent<EnemyMovement>();
        character = FindFirstObjectByType<CharacterManager>();
        status = GetComponent<EnemyStatus>();

        if (character == null)
        {
            Debug.LogWarning("No player found");
            Destroy(gameObject);
        }
        else
            playerTransform = character.transform; 

        Spawn();

        boxDetectionRadius = playerDetectionRadius;

    }

    protected virtual void Update()
    {
        if (!attacksEnabled) return;

        ChangeDirections();

        DetectSurvivorBox();

        if (detectedBox != null && CanAttack())
        {
            AttackBox();
        }
    }

    protected virtual void ChangeDirections()
    {
        if (playerTransform != null)
        {
            if (playerTransform.position.x > transform.position.x)
                _spriteRenderer.flipX = true;
            else
                _spriteRenderer.flipX = false;
        }
    }

    protected bool CanAttack() => _spriteRenderer.enabled;

    protected virtual void Attack()
    {
        if (!attacksEnabled) return;

        isCriticalHit = false;
        attackTimer = 0;

        if(canPerformCriticalHit)
        {
            float enemyCriticalHitPercent = UnityEngine.Random.Range(0, 5) / 100;

            if (enemyCriticalHitPercent >= CharacterStats.Instance.GetStatValue(Stat.CritResist))
            {
                isCriticalHit = true;

                if (isCriticalHit)
                {
                    character.TakeDamage(damage * 2);
                }
           
            }
            else
            {
                character.TakeDamage(damage);
            }
        }
        else
        {
            character.TakeDamage(damage);
        }
    }

    public virtual void TakeDamage(int _damage, bool _isCriticalHit)
    {
        if (isInvincible || this == null || gameObject == null)
            return;

        int realDamage = Mathf.Min(_damage, health);
        health -= realDamage;

        OnDamageTaken?.Invoke(_damage, transform.position, _isCriticalHit);

        if (health <= 0 && this != null && gameObject != null)
            Die();
    }

    public void ApplyLifeDrain(int damage, float duration, float interval)
    {
        StatusEffect drainEffect = new StatusEffect(StatusEffectType.Drain, duration, damage, interval);
        status.ApplyEffect(drainEffect);
    }

    protected virtual void Die()
    {
        if (this == null || gameObject == null)
            return;

        OnDeath?.Invoke(transform.position);
        OnEnemyKilled?.Invoke();

        // Debug before calling increment
        Debug.Log("MissionManager.Increment called with MissionType.enemiesPopped, 1");


        MissionManager.Increment(MissionType.enemiesPopped, 1);
        DieAfterWave();
    }

    public void DieAfterWave()
    {
        deathParticles.transform.SetParent(null);
        deathParticles.Play();

        Destroy(gameObject);
    }

    private void Spawn()
    {
        SetRenderersVisibility(false);

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

        if(movement != null)
            movement.StorePlayer(character);

        OnSpawnCompleted?.Invoke();
    }

    private void DetectSurvivorBox()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, boxDetectionRadius);
        foreach (Collider2D collider in colliders)
        {
            if (collider.CompareTag("SurvivorBox"))
            {
                detectedBox = collider.GetComponent<SurvivorBox>();
                return;
            }
        }
        detectedBox = null;
    }

    private void AttackBox()
    {
        if (detectedBox != null)
        {
            attackTimer -= Time.deltaTime;
            if (attackTimer <= 0)
            {
                detectedBox.TakeDamage(damage);
                attackTimer = 1f; 
            }
        }
    }



#region STATUS EFFECT FUNCTIONS
    public void ModifyAccuracy(float modifier)
    {
        accuracyModifier = modifier;
        Debug.Log($"Enemy accuracy modified to {accuracyModifier * 100}%.");
    }

    public void ModifyDamage(float modifier)
    {
        damageModifier = modifier;
        Debug.Log($"Enemy damage modified to {damageModifier * 100}%.");
    }

    public void DisableAttacks()
    {
        attacksEnabled = false;
        Debug.Log("Enemy attacks disabled.");
    }

    public void EnableAttacks()
    {
        attacksEnabled = true;
        Debug.Log("Enemy attacks enabled.");
    }

    public void DisableAbilities()
    {
        // Logic to disable enemy-specific abilities
        Debug.Log("Enemy abilities disabled.");
    }

    public void EnableAbilities()
    {
        // Logic to re-enable enemy-specific abilities
        Debug.Log("Enemy abilities enabled.");
    }

    public void SetTargetToOtherEnemies()
    {
       playerTransform = null;

        Enemy[] allEnemies = FindObjectsByType<Enemy>(FindObjectsSortMode.None);
        List<Enemy> validTargets = new List<Enemy>();
        foreach (var enemy in allEnemies)
        {
            if (enemy != this && enemy != null)
            {
                validTargets.Add(enemy);
            }
        }

        if (validTargets.Count > 0)
        {
            int randomIndex = UnityEngine.Random.Range(0, validTargets.Count);
            Transform newTarget = validTargets[randomIndex].transform;

            Debug.Log($"Enemy target set to: {newTarget.name}");
            movement?.SetTarget(newTarget);
        }
        else
        {
            Debug.Log("No other enemies available to target.");
        }
    }

    public void ResetTarget()
    {
        if (character != null)
        {
            playerTransform = character.transform;
            movement?.SetTarget(playerTransform);
            Debug.Log("Enemy target reset to the player.");
        }
        else
        {
            Debug.LogWarning("Player character not found; cannot reset target.");
        }
    }

#endregion
    private void OnDrawGizmos()
    {
        if (!showGizmos)
            return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, playerDetectionRadius);
        Gizmos.color = Color.yellow; 
        Gizmos.DrawWireSphere(transform.position, boxDetectionRadius);
    }

    public Vector2 GetCenter() => (Vector2)transform.position + _collider.offset;
}
