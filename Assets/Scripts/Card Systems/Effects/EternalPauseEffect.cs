using UnityEngine;

public class EternalPauseEffect : ICardEffect
{
    public void Activate(float duration)
    {
        foreach (Enemy enemy in Object.FindObjectsByType<Enemy>(FindObjectsSortMode.None))
           enemy.GetComponent<EnemyMovement>()?.DisableMovement(duration);
    }

    public void Disable()
    {
        foreach (Enemy enemy in Object.FindObjectsByType<Enemy>(FindObjectsSortMode.None))
            enemy.GetComponent<EnemyMovement>()?.EnableMovement();
    }

    
}
