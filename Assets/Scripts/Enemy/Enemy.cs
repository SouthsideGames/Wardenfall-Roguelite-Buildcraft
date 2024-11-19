using System;
using System.Collections;
using System.Collections.Generic;
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
    [SerializeField] protected SpriteRenderer _spriteRenderer;
    [SerializeField] private SpriteRenderer spawnIndicator;
    [SerializeField] private Collider2D _collider;
    protected bool hasSpawned = false;

    [Header("ATTACK:")]
    [SerializeField] protected int damage;
    [SerializeField] protected float playerDetectionRadius;
    protected float attackTimer;
    private bool isCriticalHit;

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

    [Header("EFFECTS:")]
    [SerializeField] protected ParticleSystem deathParticles;

    [Header("DEBUG:")]
    [SerializeField] private bool showGizmos;

    private EnemyStatus status;
    private Transform playerTransform;

    // Start is called before the first frame update
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

    }

    protected virtual void Update()
    {
        if (playerTransform != null)
        {
            
            if (playerTransform.position.x > transform.position.x)
                _spriteRenderer.flipX = true; 
            else
                _spriteRenderer.flipX = false;
        }
    }

    // Update is called once per frame
    protected bool CanAttack()
    {
        return _spriteRenderer.enabled;
    }

    protected virtual void Attack()
    {
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
    
        if (isInvincible)
            return;

        int realDamage = Mathf.Min(_damage, health);
        health -= realDamage;

        OnDamageTaken?.Invoke(_damage, transform.position, _isCriticalHit);


        if (health <= 0)
            Die();

    }

    public void ApplyLifeDrain(float duration)
    {
        status.ApplyLifeDrain(duration);
    }

    protected virtual void Die()
    {
        OnDeath?.Invoke(transform.position);
        OnEnemyKilled?.Invoke();
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

    private void OnDrawGizmos()
    {
        if (!showGizmos)
            return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, playerDetectionRadius);
    }

    public Vector2 GetCenter()
    {
        return (Vector2)transform.position + _collider.offset;
    }
}
