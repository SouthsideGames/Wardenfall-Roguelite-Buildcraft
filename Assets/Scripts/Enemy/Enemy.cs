using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    [Header("ACTIONS:")]
    public static Action<int, Vector2, bool> onDamageTaken;
    public static Action<Vector2, int> OnDeath;
    public static Action OnEnemyKilled;

    [Header("ELEMENTS:")]
    protected CharacterManager character;
    protected EnemyMovement movement;
    [SerializeField] protected SpriteRenderer _sr;
    [SerializeField] private SpriteRenderer spawnIndicator;
    [SerializeField] private Collider2D _col;
    protected bool hasSpawned = false;

    [Header("LEVELS:")]
    [SerializeField] private int level;

    [Header("ATTACK:")]
    [SerializeField] protected int damage;
    [SerializeField] protected float playerDetectionRadius;
    protected float attackTimer;
    private bool isCriticalHit;

    [Header("HEALTH:")]
    public int maxHealth;
    public int health;
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

    // Start is called before the first frame update
    protected virtual void Start()
    {
        health = maxHealth;

        movement = GetComponent<EnemyMovement>();
        character = FindFirstObjectByType<CharacterManager>();

        if (character == null)
        {
            Debug.LogWarning("No player found");
            Destroy(gameObject);
        }

        Spawn();

    }

    // Update is called once per frame
    protected bool CanAttack()
    {
        return _sr.enabled;
    }

    public virtual void TakeDamage(int _damage, bool _isCriticalHit)
    {
    
        if (isInvincible)
            return;

        int realDamage = Mathf.Min(_damage, health);
        health -= realDamage;

        onDamageTaken?.Invoke(_damage, transform.position, _isCriticalHit);


        if (health <= 0)
            Die();

    }


    protected virtual void Die()
    {
        OnDeath?.Invoke(transform.position, level);
        OnEnemyKilled?.Invoke();
        DieAfterWave();
    }

    public void DieAfterWave()
    {
        deathParticles.transform.SetParent(null);
        deathParticles.Play();

        Destroy(gameObject);
    }

    private void OnDrawGizmos()
    {
        if (!showGizmos)
            return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, playerDetectionRadius);
    }

    private void Spawn()
    {
        SetRenderersVisibility(false);

        Vector3 targetScale = spawnIndicator.transform.localScale * spawnSize;
        LeanTween.scale(spawnIndicator.gameObject, targetScale, spawnTime)
            .setLoopPingPong(numberOfLoops)
            .setOnComplete(ShowEnemy);

    }

    protected virtual void ShowEnemy()
    {
        SetRenderersVisibility(true);
        hasSpawned = true;
        _col.enabled = true;
        movement.StorePlayer(character);
    }

    private void SetRenderersVisibility(bool visibility)
    {
        _sr.enabled = visibility;
        spawnIndicator.enabled = !visibility;
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

   

}
