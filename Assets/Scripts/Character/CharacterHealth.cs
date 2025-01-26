using System;
using TMPro;
using UnityEngine;
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

    private void Awake() => Enemy.OnDamageTaken += EnemyDamageCallback;

    private void OnDestroy() => Enemy.OnDamageTaken -= EnemyDamageCallback;

    private void Update()
    {
        if(health < maxHealth)
           RecoverHealth();
    }

    public void SetDamageAbsorption(int percentage) => damageAbsorption = Mathf.Clamp(percentage, 0, 100);
    public void TakeDamage(int _damage)
    {

        if(ShouldDodge())
        {
            OnDodge?.Invoke(transform.position);  
            return;
        }
          
        float absorbedDamage = _damage * (damageAbsorption / 100f);
        float actualDamage = _damage - absorbedDamage;

        health -= Mathf.Min(actualDamage, health);

        UpdateHealthUI();

        if(health <= 0 )
            Die();
       

    }

    public void Heal(int _damage) => health += _damage; 

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
         var secondLifeCard = CharacterManager.Instance.deck.GetEquippedCards()
        .Find(card => card.EffectType == CardEffectType.Support_SecondLife);

        if (secondLifeCard != null && secondLifeCard.IsAutoActivated && !CardEffect.Instance.IsEffectActive(secondLifeCard.EffectType))
        {
            CardEffect.Instance.ActivateEffect(secondLifeCard.EffectType, 0, secondLifeCard);
            return; 
        }

        OnCharacterDeath?.Invoke();

        if (GameModeManager.Instance.CurrentGameMode == GameMode.Survival)
        {
            GameManager.Instance.SetGameState(GameState.SurvivalStageCompleted);
        }
        else
        {
            GameManager.Instance.SetGameState(GameState.GameOver);
        }
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

    public void OnCharacterDeathMission(GameMode _gameMode)
    {
        if ((MissionType)_gameMode == MissionType.waveBasedPlayed)
            MissionManager.Increment(MissionType.waveBasedPlayed, 1);
        else if ((MissionType)_gameMode == MissionType.survivalPlayed)
            MissionManager.Increment(MissionType.survivalPlayed, 1);
        else if ((MissionType)_gameMode == MissionType.bossRushPlayed)
            MissionManager.Increment(MissionType.bossRushPlayed, 1);
    }

}
