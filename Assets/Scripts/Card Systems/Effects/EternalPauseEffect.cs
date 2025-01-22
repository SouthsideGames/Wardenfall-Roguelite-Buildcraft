using UnityEngine;

public class EternalPauseEffect : ICardEffect
{
    public void Activate(float duration)
    {
        foreach (Enemy enemy in UnityEngine.Object.FindObjectsByType<Enemy>(FindObjectsSortMode.None))
        {
            enemy.GetComponent<EnemyMovement>()?.DisableMovement(duration);
        }
        Debug.Log("Eternal Pause activated: All movement stopped.");
    }

    public void Disable()
    {
        foreach (Enemy enemy in UnityEngine.Object.FindObjectsByType<Enemy>(FindObjectsSortMode.None))
        {
            enemy.GetComponent<EnemyMovement>()?.EnableMovement();
        }
        Debug.Log("Eternal Pause disabled: Movement resumed.");
    }

    
}
