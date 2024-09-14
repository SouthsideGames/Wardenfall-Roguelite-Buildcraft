using System;
using UnityEngine;

[RequireComponent(typeof(EnemyMovement))]
[RequireComponent(typeof(RangedEnemyAttack))]
public class RangedEnemy : MonoBehaviour
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
    private RangedEnemyAttack attack;

    [Header("Health")]
    [SerializeField] private int maxHealth;
    private int health;
    
    [Header("Spawn Values")]
    [SerializeField] private float spawnSize = 1.2f;
    [SerializeField] private float spawnTime = .3f;
    [SerializeField] private int numberOfLoops = 4;

    [Header("Attack")]
    [SerializeField] private float playerDetectionRadius;

    [Header("Effects")]
    [SerializeField] private ParticleSystem deathParticles;

    [Header("Debug")]
    [SerializeField] private bool showGizmos;
    // Start is called before the first frame update
    void Start()
    {
        health = maxHealth;
        movement = GetComponent<EnemyMovement>();   
        attack = GetComponent<RangedEnemyAttack>(); 
        player = FindFirstObjectByType<PlayerManager>();

        attack.StorePlayer(player);

        if(player == null)
        {
            Debug.LogWarning("No player found");
            Destroy(gameObject);    
        }

        Spawn();

    }

    void Update()
    {
        if(!_sr.enabled)
            return;

        AttackLogic();

    }

    private void AttackLogic()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);

        if(distanceToPlayer <= playerDetectionRadius)
            TryAttack();
        else
            movement.FollowPlayer();
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



    private void TryAttack()
    {
       attack.AutoAim();
    }

    private void SetRenderersVisibility(bool visibility)
    {
        _sr.enabled = visibility;
        spawnIndicator.enabled = !visibility;
    }


    public void TakeDamage(int _damage)
    {
        int realDamage = Mathf.Min(_damage, health);    
        health -= realDamage;  

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
