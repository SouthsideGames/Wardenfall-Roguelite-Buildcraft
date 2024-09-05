using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Elements")]
    private Player player;
    [SerializeField] private SpriteRenderer _sr;
    [SerializeField] private SpriteRenderer spawnIndicator;
    private bool hasSpawned = false;
    
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
        player = FindFirstObjectByType<Player>();

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
        _sr.enabled = false;
        spawnIndicator.enabled = true;

        Vector3 targetScale = spawnIndicator.transform.localScale * spawnSize;
        LeanTween.scale(spawnIndicator.gameObject, targetScale, spawnTime)
            .setLoopPingPong(numberOfLoops)
            .setOnComplete(ShowEnemy);

    }

    private void ShowEnemy()
    {
        _sr.enabled = true;
        spawnIndicator.enabled = false;

        hasSpawned = true;
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

    private void Attack()
    {
        Debug.Log("Dealing " + damage + " damage to the player...");
        attackTimer = 0;
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
