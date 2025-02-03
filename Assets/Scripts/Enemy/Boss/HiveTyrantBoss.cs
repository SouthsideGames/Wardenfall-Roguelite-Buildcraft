using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HiveTyrantBoss : Enemy
{
    [Header("HIVE TYRANT")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float dashSpeed = 10f;
    [SerializeField] private float dashCooldown = 3f;
    [SerializeField] private float spawnInterval = 5f;
    [SerializeField] private GameObject stingletPrefab; 
    [SerializeField] private int maxMinions = 3; 

    [Header("ADD. ELEMENTS:")]
    [SerializeField] private Slider healthBar;
    [SerializeField] private TextMeshProUGUI healthText;

    private int activeMinions = 0;
    private EnemyMovement enemyMovement;
    private Vector3 originalScale;
    private bool isDashing;

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
        originalScale = transform.localScale;

        StartCoroutine(SpawnMinions()); 
        StartCoroutine(DashRoutine());
    }

    protected override void Update()
    {
        base.Update();  

        UpdateHealthUI();

        if (!hasSpawned || isDashing) return;

        enemyMovement.FollowCurrentTarget();
    }

    private IEnumerator DashRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(dashCooldown);

            isDashing = true;
            enemyMovement.DisableMovement(0.6f);

            transform.localScale = new Vector3(originalScale.x * 1.5f, originalScale.y * 0.7f, originalScale.z);
            yield return new WaitForSeconds(0.2f);

            Vector2 dashDirection = (playerTransform.position - transform.position).normalized;
            float elapsedTime = 0f;
            Vector2 startPos = transform.position;
            Vector2 dashPos = startPos + dashDirection * 3f;

            while (elapsedTime < 0.2f)
            {
                transform.position = Vector2.Lerp(startPos, dashPos, elapsedTime / 0.2f);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            transform.position = dashPos;

            transform.localScale = new Vector3(originalScale.x * 0.7f, originalScale.y * 1.3f, originalScale.z);
            yield return new WaitForSeconds(0.1f);

            transform.localScale = originalScale;
            isDashing = false;
        }
    }

    private IEnumerator SpawnMinions()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);

            if (activeMinions < maxMinions)
                SpawnMinion();
        }
    }

    private void SpawnMinion() => Instantiate(stingletPrefab, transform.position, Quaternion.identity);

    
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
