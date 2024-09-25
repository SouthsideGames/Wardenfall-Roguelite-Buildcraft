using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CharacterHealth : MonoBehaviour
{
    [Header("ELEMENTS:")]
    [SerializeField] private Slider healthBar;
    [SerializeField] private TextMeshProUGUI healthText;


    [Header("SETTINGS:")]
    [SerializeField] private int maxHealth;
    private int health;

    private void Start()
    {
        health = maxHealth;
        UpdateHealthUI();
    }

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
}
