using UnityEngine;

public class BladeStormEffect : ICardEffect
{
    private GameObject bladePrefab;
    private int bladeCount;
    private float spawnRadius;

    public BladeStormEffect(GameObject bladePrefab, int bladeCount, float spawnRadius)
    {
        this.bladePrefab = bladePrefab;
        this.bladeCount = bladeCount;
        this.spawnRadius = spawnRadius;
    }

    public void Activate(float duration)
    {
        for (int i = 0; i < bladeCount; i++)
        {
            Vector2 spawnPosition = (Vector2)CharacterManager.Instance.transform.position + Random.insideUnitCircle * spawnRadius;
            GameObject blade = Object.Instantiate(bladePrefab, spawnPosition, Quaternion.identity);
        }

        Debug.Log($"Blade Storm activated: {bladeCount} blades spawned.");
    }

    public void Disable()
    {
        Debug.Log("Blade Storm does not require a disable phase.");
    }
}
