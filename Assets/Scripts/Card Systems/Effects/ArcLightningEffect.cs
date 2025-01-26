using UnityEngine;

public class ArcLightningEffect : ICardEffect
{
    private GameObject lightningBoltPrefab;
    private CardSO cardSO;  

    public ArcLightningEffect(GameObject _lightningBoltPrefab, CardSO _cardSO)
    {
        lightningBoltPrefab = _lightningBoltPrefab;
        cardSO = _cardSO;  
    }

    public void Activate(float duration)
    {
        Enemy[] enemies = Object.FindObjectsByType<Enemy>(FindObjectsSortMode.None);
        if (enemies.Length == 0)
        {
            Debug.LogWarning("No enemies available for Arc Lightning.");
            return;
        }

        Enemy startTarget = enemies[Random.Range(0, enemies.Length)];
        Vector2 startPosition = startTarget.transform.position;

        GameObject lightningBolt = Object.Instantiate(lightningBoltPrefab, startPosition, Quaternion.identity);
        lightningBolt.GetComponent<LightningBolt>().Activate(startPosition, cardSO);
    }

    public void Disable()
    {
    }

}
