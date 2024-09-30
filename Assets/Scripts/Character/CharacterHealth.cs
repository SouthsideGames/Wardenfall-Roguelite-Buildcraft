using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CharacterHealth : MonoBehaviour, ICharacterStats
{
    [Header("ELEMENTS:")]
    [SerializeField] private Slider healthBar;
    [SerializeField] private TextMeshProUGUI healthText;


    [Header("SETTINGS:")]
    [SerializeField] private int baseMaxHealth;
    private int maxHealth;
    private int health;

    public void TakeDamage(int _damage)
    {
        int realDamage = Mathf.Min(_damage, health);    
        health -= realDamage;  

        UpdateHealthUI();

        if(health <= 0 )
            Die();

    }

    private void Die() => GameManager.Instance.SetGameState(GameState.GAMEOVER);

    private void UpdateHealthUI()
    {
        float healthBarValue = (float)health / maxHealth;
        healthBar.value = healthBarValue;   
        healthText.text = health + " / " + maxHealth;
    }

    public void UpdateStats(CharacterStatsManager _characterStatsManager)
    {
        float addedHealth = _characterStatsManager.GetStatValue(CharacterStat.MaxHealth);
        maxHealth = baseMaxHealth + (int)addedHealth;
        maxHealth = Mathf.Max(maxHealth, 1);

        health = maxHealth;
        UpdateHealthUI();

    }
}
