using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    [Header("Actions")]
    public static Action<int, Vector2> onDamageTaken;

    [Header("Elements")]
    protected PlayerManager player;
    protected EnemyMovement movement;
    [SerializeField] protected SpriteRenderer _sr;
    [SerializeField] private SpriteRenderer spawnIndicator;
    [SerializeField] private Collider2D _col;
    protected bool hasSpawned = false;

    [Header("Attack")]
    [SerializeField] protected float playerDetectionRadius;

    [Header("Health")]
    public int maxHealth;
    [HideInInspector] public int health;
    [HideInInspector] public bool isInvincible = false;

    [Header("Spawn Values")]
    [SerializeField] private float spawnSize = 1.2f;
    [SerializeField] private float spawnTime = .3f;
    [SerializeField] private int numberOfLoops = 4;

    [Header("Effects")]
    [SerializeField] protected ParticleSystem deathParticles;

    [Header("Debug")]
    [SerializeField] private bool showGizmos;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        health = maxHealth;

        movement = GetComponent<EnemyMovement>();
        player = FindFirstObjectByType<PlayerManager>();

        if (player == null)
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

    public void TakeDamage(int _damage)
    {
    
        if (isInvincible)
            return;

        int realDamage = Mathf.Min(_damage, health);
        health -= realDamage;

        onDamageTaken?.Invoke(_damage, transform.position);


        if (health <= 0)
            Die();

    }


    protected virtual void Die()
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
        movement.StorePlayer(player);
    }

    private void SetRenderersVisibility(bool visibility)
    {
        _sr.enabled = visibility;
        spawnIndicator.enabled = !visibility;
    }


}
