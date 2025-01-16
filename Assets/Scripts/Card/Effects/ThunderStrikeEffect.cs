using UnityEngine;

public class ThunderStrikeEffect : ICardEffect
{
     private GameObject thunderboltPrefab;

    public ThunderStrikeEffect(GameObject thunderboltPrefab)
    {
        this.thunderboltPrefab = thunderboltPrefab;
    }

    public void Activate(float duration)
    {
        Vector2 targetPosition = GetRandomEnemyPosition();
        if (targetPosition == Vector2.zero)
        {
            Debug.LogWarning("No enemies available to target.");
            return;
        }

        GameObject thunderbolt = Object.Instantiate(thunderboltPrefab, targetPosition, Quaternion.identity);
        thunderbolt.GetComponent<Thunderbolt>().Strike(targetPosition, 500);
        Debug.Log("Thunder Strike activated!");
    }

    public void Disable()
    {
        Debug.Log("Thunder Strike does not require a disable phase.");
    }

    private Vector2 GetRandomEnemyPosition()
    {
        Enemy[] enemies = (Enemy[])Object.FindObjectsOfType(typeof(Enemy));
        if (enemies.Length == 0)
            return Vector2.zero;

        Enemy targetEnemy = enemies[Random.Range(0, enemies.Length)];
        return targetEnemy.transform.position;
    }
}
