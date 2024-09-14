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
    [SerializeField] protected SpriteRenderer spawnIndicator;
    [SerializeField] protected Collider2D _col;
    protected bool hasSpawned = false;

    [Header("Attack")]
    [SerializeField] protected float playerDetectionRadius;

    [Header("Health")]
    [SerializeField] private int maxHealth;
    private int health;

    [Header("Spawn Values")]
    [SerializeField] protected float spawnSize = 1.2f;
    [SerializeField] protected float spawnTime = .3f;
    [SerializeField] protected int numberOfLoops = 4;

    [Header("Effects")]
    [SerializeField] private ParticleSystem deathParticles;

    [Header("Debug")]
    [SerializeField] private bool showGizmos;

    // Start is called before the first frame update
    void Start()
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
    void Update()
    {
         if(!_sr.enabled)
            return;
    }

    public void TakeDamage(int _damage)
    {
        int realDamage = Mathf.Min(_damage, health);
        health -= realDamage;

        onDamageTaken?.Invoke(_damage, transform.position);


        if (health <= 0)
            Die();

    }


    private void Die()
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

    private void ShowEnemy()
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
