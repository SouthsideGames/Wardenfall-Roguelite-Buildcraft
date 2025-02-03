using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TwinfangBoss : Enemy
{
     [Header("TWINFANG")]
    [SerializeField] private float moveSpeed = 1.5f;   // Slow movement speed
    [SerializeField] private float lungeSpeed = 8f;   // Speed of the lunge attack
    [SerializeField] private float retreatSpeed = 6f; // Speed when jumping back
    [SerializeField] private float attackRange = 3f;
    [SerializeField] private float attackCooldown = 2f;

    [Header("ADD. ELEMENTS:")]
    [SerializeField] private Slider healthBar;
    [SerializeField] private TextMeshProUGUI healthText;

    private EnemyMovement enemyMovement;
    private bool isAttacking;
    private float attackCooldownTimer;
    private Vector3 originalScale;

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
        enemyMovement = GetComponent<EnemyMovement>(); // Reference movement script
        originalScale = transform.localScale;
    }

    protected override void Update()
    {
        base.Update();  

        UpdateHealthUI();

        if (!hasSpawned || isAttacking) return;

        attackCooldownTimer -= Time.deltaTime;

        if (enemyMovement != null && !isAttacking)
        {
            enemyMovement.FollowCurrentTarget();
        }

        if (Vector2.Distance(transform.position, playerTransform.position) <= attackRange && attackCooldownTimer <= 0)
        {
            StartCoroutine(LungeAttack());
        }
    }

    private IEnumerator LungeAttack()
    {
        isAttacking = true;
        enemyMovement.DisableMovement(0.6f); // Disable movement during attack

        Vector2 attackDirection = (playerTransform.position - transform.position).normalized;

        // **1. Stretch before lunging**
        transform.localScale = new Vector3(originalScale.x * 1.5f, originalScale.y * 0.7f, originalScale.z);
        yield return new WaitForSeconds(0.2f);

        // **2. Quick lunge forward**
        float elapsedTime = 0f;
        Vector2 startPos = transform.position;
        Vector2 lungePos = startPos + attackDirection * attackRange;

        while (elapsedTime < 0.2f)
        {
            transform.position = Vector2.Lerp(startPos, lungePos, elapsedTime / 0.2f);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = lungePos;

        // **3. Squash effect after landing**
        transform.localScale = new Vector3(originalScale.x * 0.7f, originalScale.y * 1.3f, originalScale.z);
        yield return new WaitForSeconds(0.1f);

        // **4. Jump backward to reset distance**
        Vector2 retreatPos = (Vector2)transform.position - attackDirection * (attackRange * 0.75f);

        elapsedTime = 0f;
        while (elapsedTime < 0.2f)
        {
            transform.position = Vector2.Lerp(lungePos, retreatPos, elapsedTime / 0.2f);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = retreatPos;

        // **5. Reset**
        transform.localScale = originalScale;
        attackCooldownTimer = attackCooldown;
        isAttacking = false;
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