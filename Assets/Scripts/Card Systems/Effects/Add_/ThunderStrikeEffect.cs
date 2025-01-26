using UnityEngine;

public class ThunderStrikeEffect : ICardEffect
{
    private GameObject thunderboltPrefab;

    private CardSO cardSO;

    public ThunderStrikeEffect(GameObject _thunderboltPrefab, CardSO _cardSO)
    {
        thunderboltPrefab = _thunderboltPrefab;
        cardSO = _cardSO;  
    }

    public void Activate(float duration)
    {
        Vector2 targetPosition = GetRandomEnemyPosition();
        if (targetPosition == Vector2.zero)
            return;

        GameObject thunderbolt = Object.Instantiate(thunderboltPrefab, targetPosition, Quaternion.identity);
        thunderbolt.GetComponent<Thunderbolt>().Strike(targetPosition, cardSO);
    }

    public void Disable()
    {
    }

    private Vector2 GetRandomEnemyPosition()
    {
        Enemy[] enemies = Object.FindObjectsByType<Enemy>(FindObjectsSortMode.None);
        if (enemies.Length == 0)
            return Vector2.zero;

        Enemy targetEnemy = enemies[Random.Range(0, enemies.Length)];
        return targetEnemy.transform.position;
    }

}
