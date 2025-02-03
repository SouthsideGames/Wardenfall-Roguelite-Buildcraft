using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GravulonBoss : Enemy
{
    [Header("Gravulon Settings")]
    [SerializeField] private float moveSpeed = 1.5f;
    [SerializeField] private float rollSpeed = 6f;
    [SerializeField] private float rollCooldown = 3f;
    [SerializeField] private float rollDuration = 0.8f;
    [SerializeField] private int rollDamage = 20;

    [Header("Shockwave Slam")]
    [SerializeField] private float slamRange = 3f; // Radius of the shockwave
    [SerializeField] private int slamDamage = 25; // Damage dealt by the shockwave
    [SerializeField] private float stunDuration = 1.5f; // How long enemies are stunned
    [SerializeField] private float knockbackForce = 5f; // How much the player gets knocked back
    [SerializeField] private float slamCooldown = 6f;

    
    [Header("ADD. ELEMENTS:")]
    [SerializeField] private Slider healthBar;
    [SerializeField] private TextMeshProUGUI healthText;

    private EnemyMovement enemyMovement;
    private bool isRolling;
    private bool isSlamming;

    private void Awake()
    {
        healthBar.gameObject.SetActive(false);
        OnSpawnCompleted += SpawnCompletedCallback;
        OnDamageTaken += DamageTakenCallback;
    }

    private void OnDestroy()
    {
        OnSpawnCompleted -= SpawnCompletedCallback;
        OnDamageTaken -= DamageTakenCallback;
    }

   protected override void Start()
    {
        base.Start();
        enemyMovement = GetComponent<EnemyMovement>();
        StartCoroutine(SlamRoutine());
    }

    protected override void Update()
    {
        if (!hasSpawned || isRolling || isSlamming) return;

        enemyMovement.FollowCurrentTarget();
    }

    private IEnumerator SlamRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(slamCooldown);

            if (!hasSpawned || isSlamming) continue;

            isSlamming = true;
            enemyMovement.DisableMovement(1.5f); // Temporarily stop movement

            // **1. Prepare for the Slam**
            transform.localScale = new Vector3(transform.localScale.x * 1.2f, transform.localScale.y * 0.8f, transform.localScale.z);
            yield return new WaitForSeconds(0.5f); // Short delay before slamming

            // **2. Slam Down**
            transform.localScale = new Vector3(transform.localScale.x * 0.8f, transform.localScale.y * 1.2f, transform.localScale.z);
            DealShockwaveDamage(); // Apply the area-of-effect damage
            yield return new WaitForSeconds(0.3f);

            // **3. Recover & Reset**
            transform.localScale = Vector3.one;
            isSlamming = false;
            enemyMovement.EnableMovement();
        }
    }

    private void DealShockwaveDamage()
    {
        Collider2D[] hitObjects = Physics2D.OverlapCircleAll(transform.position, slamRange);

        foreach (Collider2D hit in hitObjects)
        {
            if (hit.CompareTag("Player"))
            {
                character.TakeDamage(slamDamage);
                Vector2 knockbackDirection = (hit.transform.position - transform.position).normalized;
                Debug.Log("Gravulon slammed the player!");
            }
            else if (hit.CompareTag("Enemy"))
            {
                Enemy enemy = hit.GetComponent<Enemy>();
                if (enemy != null)
                {
                    enemy.TakeDamage(slamDamage, false);
                    movement.DisableMovement(stunDuration);
                    Debug.Log($"Gravulon stunned enemy: {hit.name}");
                }
            }
            else if (hit.CompareTag("SurvivorBox"))
            {
                SurvivorBox box = hit.GetComponent<SurvivorBox>();
                if (box != null)
                {
                    box.TakeDamage(slamDamage);
                    Debug.Log("Gravulon damaged the Survivor Box!");
                }
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, slamRange);
    }
    
    private void SpawnCompletedCallback()
    {
        UpdateHealthUI();
        healthBar.gameObject.SetActive(true);

    }


    private void UpdateHealthUI()
    {
        healthBar.value = (float)health / maxHealth;
        healthText.text = $"{health} / {maxHealth}";
    }

    private void DamageTakenCallback(int _damage, Vector2 _position, bool _isCriticalHit) => UpdateHealthUI();
}
