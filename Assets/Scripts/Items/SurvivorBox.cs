using UnityEngine;
using TMPro;

/// <summary>
/// A destructible box with a timer that spawns either a CollectableWeapon or CollectableObject
/// when the timer expires, or gets destroyed if health reaches zero before the timer ends.
/// Displays timer and health using TMPro.
/// </summary>
public class SurvivorBox : MonoBehaviour
{
    [Header("ELEMENTS:")]
    [SerializeField] private GameObject collectableObjectPrefab;
    [SerializeField] private GameObject collectableWeaponPrefab;

    [Header("UI:")]
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI healthText;

    [Header("STATS:")]
    [SerializeField] private int maxHealth = 10;
    [SerializeField] private float timerDuration = 5f;
    [SerializeField] private float activationRange = 5f;

    [Header("COLORS:")]
    [SerializeField] private Color fullHealthColor = Color.green;
    [SerializeField] private Color lowHealthColor = Color.red;

    private int currentHealth;
    private float timer;
    private bool isActive = false;
    private bool isPlayerInRange = false;

    private Transform playerTransform;

    private void Awake() => playerTransform = CharacterManager.Instance.transform;

    private void OnEnable() => ResetBox();

    private void Update()
    {
        if (!isActive) return;

        CheckPlayerDistance();

        if (isPlayerInRange)
        {
            // Timer countdown only when player is in range
            timer -= Time.deltaTime;
            UpdateUI();

            if (timer <= 0)
            {
                SpawnRandomItem();
                DestroyBox();
            }
        }
    }


    public void Activate()
    {
        ResetBox();
        isActive = true;
    }

    private void ResetBox()
    {
        currentHealth = maxHealth;
        timer = timerDuration;
        isActive = false;
        isPlayerInRange = false;

        UpdateUI();
        UpdateColor();
    }

    public void TakeDamage(int damage)
    {
        if (!isActive) return;

        currentHealth -= damage;
        UpdateUI();
        UpdateColor();

        if (currentHealth <= 0)
        {
            DestroyBox();
        }
    }

    private void CheckPlayerDistance()
    {
        if (playerTransform == null) return;

        float distance = Vector2.Distance(transform.position, playerTransform.position);
        isPlayerInRange = distance <= activationRange;
    }

    private void UpdateColor()
    {
        float healthPercentage = (float)currentHealth / maxHealth;
        Color color = Color.Lerp(lowHealthColor, fullHealthColor, healthPercentage);

        healthText.color = color;
    }


    private void UpdateUI()
    {
        if (timerText != null)
        {
            timerText.text = timer.ToString("F1");
        }

        if (healthText != null)
        {
            healthText.text = $" HP: {currentHealth}/{maxHealth}";
        }
    }

    private void SpawnRandomItem()
    {
        bool spawnWeapon = Random.value > 0.5f;

        if (spawnWeapon && collectableWeaponPrefab != null)
        {
            Instantiate(collectableWeaponPrefab, transform.position, Quaternion.identity);
        }
        else if (collectableObjectPrefab != null)
        {
            Instantiate(collectableObjectPrefab, transform.position, Quaternion.identity);
        }
    }

    private void DestroyBox() => gameObject.SetActive(false); 
}
