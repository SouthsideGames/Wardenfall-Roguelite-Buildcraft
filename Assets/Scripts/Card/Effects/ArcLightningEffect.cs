using UnityEngine;

public class ArcLightningEffect : ICardEffect
{
    private GameObject lightningBoltPrefab;

    public ArcLightningEffect(GameObject lightningBoltPrefab)
    {
        this.lightningBoltPrefab = lightningBoltPrefab;
    }

    public void Activate(float duration)
    {
       Enemy[] enemies = (Enemy[])Object.FindObjectsOfType(typeof(Enemy));
        if (enemies.Length == 0)
        {
            Debug.LogWarning("No enemies available for Arc Lightning.");
            return;
        }

        Enemy startTarget = enemies[Random.Range(0, enemies.Length)];
        Vector2 startPosition = startTarget.transform.position;

        GameObject lightningBolt = Object.Instantiate(lightningBoltPrefab, startPosition, Quaternion.identity);
        lightningBolt.GetComponent<LightningBolt>().Activate(startPosition, 200);
        Debug.Log("Arc Lightning activated!");
    }

    public void Disable()
    {
        Debug.Log("Arc Lightning does not require a disable phase.");
    }
}
