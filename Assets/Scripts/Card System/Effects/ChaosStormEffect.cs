using UnityEngine;

public class ChaosStormEffect : MonoBehaviour, ICardEffect
{
    [SerializeField] private GameObject lightningPrefab;
    [SerializeField] private int minStrikes = 5;
    [SerializeField] private int maxStrikes = 8;

    private float damage;

    public void Activate(CharacterManager target, CardSO card)
    {
        if (target == null || lightningPrefab == null)
        {
            Debug.LogWarning("ChaosStormEffect: Target or prefab is missing.");
            return;
        }

        damage = card.effectValue > 0 ? card.effectValue : 20f;
        TriggerLightning();
        Destroy(gameObject); // destroy effect instance after one-time use
    }

    public void Deactivate() { }

    public void Tick(float deltaTime) { }

    private void TriggerLightning()
    {
        int strikes = Random.Range(minStrikes, maxStrikes + 1);
        for (int i = 0; i < strikes; i++)
        {
            Vector2 targetPos = GetRandomEnemyOrNearbyPosition();
            GameObject strike = Instantiate(lightningPrefab, targetPos, Quaternion.identity);
            if (strike.TryGetComponent(out LightningStrike lightning))
                lightning.Initialize(damage);
        }
    }

    private Vector2 GetRandomEnemyOrNearbyPosition()
    {
        Enemy[] enemies = FindObjectsByType<Enemy>(FindObjectsSortMode.None);
        if (enemies.Length > 0)
        {
            Enemy target = enemies[Random.Range(0, enemies.Length)];
            return (Vector2)target.transform.position + Random.insideUnitCircle * 1.5f;
        }

        return new Vector2(
            Random.Range(0, Constants.arenaSize.x),
            Random.Range(0, Constants.arenaSize.y)
        );
    }
}
