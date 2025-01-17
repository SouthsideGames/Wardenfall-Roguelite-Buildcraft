using UnityEngine;

public class DeathRayEffect : ICardEffect
{
    private GameObject deathCloudPrefab;
    private CardSO cardSO;

    public DeathRayEffect(GameObject deathCloudPrefab, CardSO cardSO)
    {
        this.deathCloudPrefab = deathCloudPrefab;
        this.cardSO = cardSO;
    }

    public void Activate(float duration)
    {
        Vector2 playerPosition = CharacterManager.Instance.transform.position;

        GameObject deathCloud = Object.Instantiate(deathCloudPrefab, playerPosition, Quaternion.identity);
        DeathRay cloudScript = deathCloud.GetComponent<DeathRay>();
        cloudScript.Configure(duration);

        Debug.Log($"Death Cloud activated for {duration}s. Beams will randomly strike enemies.");
    }

    public void Disable()
    {
        Debug.Log("Death Cloud does not require a disable phase.");
    }
}
