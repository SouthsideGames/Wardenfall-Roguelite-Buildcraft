using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CharacterHealth : MonoBehaviour, ICharacterStats
{
    [Header("ACTIONS:")]
    public static Action<Vector2> OnDodge;

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
        Enemy.onDamageTaken += EnemyDamageCallback;
    }

    private void OnDestroy() 
    {
        Enemy.onDamageTaken -= EnemyDamageCallback;
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

    private void Die() => GameManager.Instance.SetGameState(GameState.GameOver);

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

    public void UpdateStats(CharacterStatsManager _characterStatsManager)
    {
        // Retrieve the MaxHealth value from the character stats manager and add it to baseMaxHealth
        float addedHealth = _characterStatsManager.GetStatValue(CharacterStat.MaxHealth);
        maxHealth = baseMaxHealth + (int)addedHealth;
        maxHealth = Mathf.Max(maxHealth, 1); // Calculate the new max health and ensure it is not less than 1

        health = maxHealth;
        UpdateHealthUI();

        armor = _characterStatsManager.GetStatValue(CharacterStat.Armor);
        lifeSteal = _characterStatsManager.GetStatValue(CharacterStat.LifeSteal) / 100;
        dodge = _characterStatsManager.GetStatValue(CharacterStat.Dodge);

        // Calculate Health Recovery Speed and ensure it is not zero (minimum of .0001f)
        healthRecoverySpeed = MathF.Max(.0001f, _characterStatsManager.GetStatValue(CharacterStat.HealthRecoverySpeed));
        healthRecoveryDuration = 1f / healthRecoverySpeed;
        healthRecoveryValue = _characterStatsManager.GetStatValue(CharacterStat.HealthRecoveryValue);

    }

}
