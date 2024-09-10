using System;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(EnemyMovement))]
public class Enemy : MonoBehaviour
{
    [Header("Actions")]
    public static Action<int, Vector2> onDamageTaken;

    [Header("Elements")]
    private PlayerManager player;
    [SerializeField] private SpriteRenderer _sr;
    [SerializeField] private SpriteRenderer spawnIndicator;
    [SerializeField] private Collider2D _col;
    private bool hasSpawned = false;
    private EnemyMovement movement;

    [Header("Health")]
    [SerializeField] private int maxHealth;
    private int health;
    [SerializeField] private TextMeshPro healthText;
    
    [Header("Spawn Values")]
    [SerializeField] private float spawnSize = 1.2f;
    [SerializeField] private float spawnTime = .3f;
    [SerializeField] private int numberOfLoops = 4;

    [Header("Attack")]
    [SerializeField] private float playerDetectionRadius;
    [SerializeField] private int damage;
    [SerializeField] private float attackRate;
    private float attackDelay;
    private float attackTimer;

    [Header("Effects")]
    [SerializeField] private ParticleSystem deathParticles;

    [Header("Debug")]
    [SerializeField] private bool showGizmos;

    // Start is called before the first frame update
    void Start()
    {
        health = maxHealth;
        healthText.text = health.ToString();
        movement = GetComponent<EnemyMovement>();   
        player = FindFirstObjectByType<PlayerManager>();

        if(player == null)
        {
            Debug.LogWarning("No player found");
            Destroy(gameObject);    
        }

        Spawn();

        attackDelay = 1f / attackRate;
        Debug.Log("Attack Delay : " + attackDelay);
    }

    void Update()
    {
         if(attackTimer >= attackDelay)    
            TryAttack();
        else
            Wait();
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

    private void Wait()
    {
        attackTimer += Time.deltaTime;
    }

    private void TryAttack()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);

        if(distanceToPlayer <= playerDetectionRadius)
            Attack();
    }

    private void SetRenderersVisibility(bool visibility)
    {
        _sr.enabled = visibility;
        spawnIndicator.enabled = !visibility;
    }

    private void Attack()
    {

        attackTimer = 0;
        player.TakeDamage(damage);
    }

    public void TakeDamage(int _damage)
    {
        int realDamage = Mathf.Min(_damage, health);    
        health -= realDamage;  

        healthText.text = health.ToString();

        onDamageTaken?.Invoke(_damage, transform.position);


        if(health <= 0)
            Die();

    }

    private void OnDrawGizmos()
    {
        if(!showGizmos)
            return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, playerDetectionRadius);
    }

    private void Die()
    {
        deathParticles.transform.SetParent(null);
        deathParticles.Play();

        Destroy(gameObject);
    }

}
