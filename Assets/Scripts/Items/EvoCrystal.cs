using SouthsideGames.DailyMissions;
using UnityEngine;

public class EvoCrystal : MonoBehaviour
{
    [SerializeField] private float maxHealth = 10f;
    private float currentHealth;

    private void Start()
    {
        currentHealth = maxHealth;
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        MissionManager.Increment(MissionType.evolveEnemies, 1);
        MissionManager.Increment(MissionType.evolveEnemies2, 1);
        MissionManager.Increment(MissionType.evolveEnemies3, 1);
        MissionManager.Increment(MissionType.evolveEnemies4, 1);
        MissionManager.Increment(MissionType.evolveEnemies5, 1);

        if (other.TryGetComponent<Enemy>(out var enemy))
        {
            if (enemy.CanEvolve())
            {
                enemy.Evolve();
                Destroy(gameObject);
            }
        }
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        if (currentHealth <= 0f)
        {
            Destroy(gameObject);
        }
    }
}
