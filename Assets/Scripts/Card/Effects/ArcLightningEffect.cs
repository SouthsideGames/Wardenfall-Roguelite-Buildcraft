using UnityEngine;

public class ArcLightningEffect : ICardEffect
{
    private GameObject lightningBoltPrefab;
    private CardSO cardSO;  

    public ArcLightningEffect(GameObject lightningBoltPrefab, CardSO _cardSO)
    {
        this.lightningBoltPrefab = lightningBoltPrefab;
        this.cardSO = _cardSO;  
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
        Debug.Log("Arc Lightning activated!");
    }

    public void Disable()
    {
        Debug.Log("Arc Lightning does not require a disable phase.");
    }
    public void ApplySynergy(float synergyBonus)
    {
        throw new System.NotImplementedException();
    }

}
