using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using SouthsideGames.DailyMissions;

public class CharacterHealth : MonoBehaviour, IStats
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
    private float maxHealth;
    private float health;
    private float armor;
    private float lifeSteal;
    private float dodge;
    private float healthRecoverySpeed;
    private float healthRecoveryValue;
    private float healthRecoveryTimer;
    private float healthRecoveryDuration;

    private void Awake()
    {
        Enemy.OnDamageTaken += EnemyDamageCallback;
    }

    private void OnDestroy() 
    {
        Enemy.OnDamageTaken -= EnemyDamageCallback;
    }

    private void Update()
    {
        if(health < maxHealth)
           RecoverHealth();
    }

    public void TakeDamage(int _damage)
    {

        if(ShouldDodge())
        {
            OnDodge?.Invoke(transform.position);  
            return;
        }
        
        
        float realDamage = _damage * Mathf.Clamp(1 - (armor / 1000), 0, 10000);
        realDamage = Mathf.Min(realDamage, health);
        health -= realDamage;  

        UpdateHealthUI();

        if(health <= 0 )
            Die();
       

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

    private void Die()
    {
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
        float clampedDodge = Mathf.Clamp(dodge, 0f, 50f);
        return UnityEngine.Random.Range(0f, 100f) < clampedDodge;
    }

    private void RecoverHealth()
    {
        healthRecoveryTimer += Time.deltaTime;

        if(healthRecoveryTimer >= healthRecoveryDuration)
        {
            healthRecoveryTimer = 0;

            float healthToAdd = Mathf.Min(healthRecoveryValue, maxHealth - health);
            health += healthToAdd;

            UpdateHealthUI();
        }
    }

    public void UpdateWeaponStats(CharacterStats _statsManager)
    {
        // Retrieve the MaxHealth value from the character stats manager and add it to baseMaxHealth
        float addedHealth = _statsManager.GetStatValue(Stat.MaxHealth);
        maxHealth = baseMaxHealth + (int)addedHealth;
        maxHealth = Mathf.Max(maxHealth, 1); // Calculate the new max health and ensure it is not less than 1

        health = maxHealth;
        UpdateHealthUI();

        armor = _statsManager.GetStatValue(Stat.Armor);
        lifeSteal = _statsManager.GetStatValue(Stat.LifeSteal) / 100;
        dodge = _statsManager.GetStatValue(Stat.Dodge);

        // Calculate Health Recovery Speed and ensure it is not zero (minimum of .0001f)
        healthRecoverySpeed = MathF.Max(.0001f, _statsManager.GetStatValue(Stat.RegenSpeed));
        healthRecoveryDuration = 1f / healthRecoverySpeed;
        healthRecoveryValue = _statsManager.GetStatValue(Stat.RegenValue);

    }

    public void OnCharacterDeathMission(GameMode _gameMode)
    {
        if ((MissionType)_gameMode == MissionType.waveBasedPlayed)
            MissionManager.Increment(MissionType.waveBasedPlayed, 1);
        else if ((MissionType)_gameMode == MissionType.survivalPlayed)
            MissionManager.Increment(MissionType.survivalPlayed, 1);
        else if ((MissionType)_gameMode == MissionType.objectiveBasedPlayed)
            MissionManager.Increment(MissionType.objectiveBasedPlayed, 1);
        else if ((MissionType)_gameMode == MissionType.bossRushPlayed)
            MissionManager.Increment(MissionType.bossRushPlayed, 1);
        else if ((MissionType)_gameMode == MissionType.endlessPlayed)
            MissionManager.Increment(MissionType.endlessPlayed, 1);


    }

}
