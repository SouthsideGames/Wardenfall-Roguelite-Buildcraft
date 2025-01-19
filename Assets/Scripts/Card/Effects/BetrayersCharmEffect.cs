using UnityEngine;

public class BetrayersCharmEffect : ICardEffect
{
    public void Activate(float duration)
    {
        Enemy[] enemies = Object.FindObjectsByType<Enemy>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
        if (enemies == null || enemies.Length == 0)
            return;

        // Select a random enemy
        Enemy selectedEnemy = enemies[UnityEngine.Random.Range(0, enemies.Length)];
        EnemyStatus status = selectedEnemy.GetComponent<EnemyStatus>();

        if (status != null)
        {
            // Apply the Confuse effect
            StatusEffect confuseEffect = new StatusEffect(StatusEffectType.Confuse, duration);
            status.ApplyEffect(confuseEffect);

            Debug.Log($"Betrayer's Charm activated: {selectedEnemy.name} is now attacking other enemies.");
        }
        else
        {
            Debug.LogWarning($"Enemy {selectedEnemy.name} does not have an EnemyStatus component.");
        }
    }

    public void Disable()
    {
        Debug.Log("Betrayer's Charm effect does not require a disable phase.");
    }
}
