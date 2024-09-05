using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EnemyMovement))]
public class Enemy : MonoBehaviour
{
    [Header("Elements")]
    private Player player;
    [SerializeField] private SpriteRenderer _sr;
    [SerializeField] private SpriteRenderer spawnIndicator;
    private bool hasSpawned = false;
    private EnemyMovement movement;
    
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
        movement = GetComponent<EnemyMovement>();   
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
