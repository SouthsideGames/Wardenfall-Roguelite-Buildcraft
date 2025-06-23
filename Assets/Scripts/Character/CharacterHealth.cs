using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using SouthsideGames.DailyMissions;

public class CharacterHealth : MonoBehaviour, IStats, IDamageable
{
    [Header("ACTIONS:")]
    public static Action<Vector2> OnDodge;
    public static Action OnCharacterDeath;

    [Header("ELEMENTS:")]
    [SerializeField] private Slider healthBar;
    [SerializeField] private TextMeshProUGUI healthText;

    [Header("SETTINGS:")]
    [SerializeField] private int baseMaxHealth;

    [Header("STATS:")]
    public float maxHealth {get; private set;}
    private float health;
    private float armor;
    private float lifeSteal;
    private float dodge;
    private float healthRecoverySpeed;
    private float healthRecoveryValue;
    private float healthRecoveryTimer;
    private float healthRecoveryDuration;
    private int damageAbsorption = 0;
    private float damageTakenMultiplier = 1f;
    public bool IsAlive => health > 0;
    public int MaxHealth => Mathf.RoundToInt(maxHealth);
    public int CurrentHealth => Mathf.RoundToInt(health);
    [HideInInspector] public bool isInvincible = false;

    #region CARDS
    private bool hasRebornTriggered = false;
    private PulseWardTracker pulseWardTracker;

    #endregion

    private void Awake()
    {
        pulseWardTracker = GetComponent<PulseWardTracker>();
        Enemy.OnDamageTaken += EnemyDamageCallback;
    }

    private void OnEnable()
    {
        CharacterStats stats = GetComponent<CharacterStats>();
        if (stats != null)
            stats.RegisterStatReceiver(this);
    }

    private void OnDisable()
    {
        CharacterStats stats = GetComponent<CharacterStats>();
        if (stats != null)
            stats.UnregisterStatReceiver(this);
    }

    private void OnDestroy() => Enemy.OnDamageTaken -= EnemyDamageCallback;

    private void Update()
    {
        if(health < maxHealth)
           RecoverHealth();
    }

    public void SetDamageAbsorption(int percentage) => damageAbsorption = Mathf.Clamp(percentage, 0, 100);

    public void TakeDamage(int _damage, bool isCritical = false)
    {

        if (isInvincible)
            return;

        if (ShouldDodge())
            return;

        pulseWardTracker.OnDamageTaken();

        float absorbedDamage = _damage * (damageAbsorption / 100f);
        float rawDamage = _damage - absorbedDamage;

        float actualDamage = rawDamage * damageTakenMultiplier;

        health -= Mathf.Min(actualDamage, health);

        UpdateHealthUI();

        if (health <= 0)
            Die();

        WaveManager.Instance?.ReportPlayerHit();
        MissionTotalDamageReceived(_damage);

    }

    public void Heal(int _damage)
    {
        if (ChallengeManager.IsActive(ChallengeMode.Hardcore))
            return;

        health = Mathf.Min(health + _damage, maxHealth);
        UpdateHealthUI();
    }

    private void EnemyDamageCallback(int _damage, Vector2 _enemyPos, bool _isCriticalHit)
    {
        if(health >= maxHealth)
            return;
        
        float lifeStealValue = _damage * lifeSteal;
        float healthToAdded = MathF.Min(lifeStealValue, maxHealth - health);

        health += healthToAdded;
        UpdateHealthUI();
    }

    public void Die()
    {

        if (!hasRebornTriggered && CharacterManager.Instance.cards.HasCard("reborn"))
        {
            hasRebornTriggered = true;
            health = maxHealth;
            UpdateHealthUI();
            return;
        }

        AudioManager.Instance?.PlayCrowdReaction(CrowdReactionType.Ouch);
        OnCharacterDeath?.Invoke();
        GameManager.Instance.SetGameState(GameState.GameOver);
    }

    private void UpdateHealthUI()
    {
        float healthBarValue = health / maxHealth;
        healthBar.value = healthBarValue;   
        healthText.text = (int)health + " / " + maxHealth;
    }

    private bool ShouldDodge()
    {
        WaveManager.Instance?.AdjustViewerScore(0.02f);
        float clampedDodge = Mathf.Clamp(dodge, 0f, 50f);
        return UnityEngine.Random.Range(0f, 100f) < clampedDodge;
    }

    private void RecoverHealth()
    {
        if (ChallengeManager.IsActive(ChallengeMode.Hardcore))
            return;

        healthRecoveryTimer += Time.deltaTime;

        if (healthRecoveryTimer >= healthRecoveryDuration)
        {
            healthRecoveryTimer = 0;

            float healthToAdd = Mathf.Min(healthRecoveryValue, maxHealth - health);
            health += healthToAdd;

            UpdateHealthUI();
        }
    }

    public void UpdateWeaponStats(CharacterStats _statsManager)
    {
        float addedHealth = _statsManager.GetStatValue(Stat.MaxHealth);
        maxHealth = baseMaxHealth + (int)addedHealth;
        maxHealth = Mathf.Max(maxHealth, 1);

        health = maxHealth;
        UpdateHealthUI();

        armor = _statsManager.GetStatValue(Stat.Armor);
        lifeSteal = _statsManager.GetStatValue(Stat.LifeSteal) / 100;
        dodge = _statsManager.GetStatValue(Stat.Dodge);

        healthRecoverySpeed = MathF.Max(.0001f, _statsManager.GetStatValue(Stat.RegenSpeed));
        healthRecoveryDuration = 1f / healthRecoverySpeed;
        healthRecoveryValue = _statsManager.GetStatValue(Stat.RegenValue);

    }

    public void SetInvincible(bool state) 
    {
        isInvincible = state;

        if (state)
        {
            CharacterManager.Instance._sr.color = new Color(1, 1, 1, .5f);
            isInvincible = true;
        }
        else
        {
            CharacterManager.Instance._sr.color = new Color(1, 1, 1, 1f);
            isInvincible = false;
        }
            
    }

    public void SetDamageTakenMultiplier(float multiplier) => damageTakenMultiplier = multiplier;

    private static void MissionTotalDamageReceived(int _damage)
    {
        MissionManager.Increment(MissionType.totalDamageReceived, _damage);
        MissionManager.Increment(MissionType.totalDamageReceived1, _damage);
        MissionManager.Increment(MissionType.totalDamageReceived2, _damage);
    }

}
