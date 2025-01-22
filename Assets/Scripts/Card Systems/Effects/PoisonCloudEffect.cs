using UnityEngine;

public class PoisonCloudEffect : ICardEffect
{
    private GameObject poisonCloudPrefab;
    private CardSO card;

    public PoisonCloudEffect(GameObject poisonCloudPrefab, CardSO _card)
    {
        this.poisonCloudPrefab = poisonCloudPrefab;
        card = _card;
    }

    public void Activate(float activeTime)
    {
        Vector2 playerPosition = CharacterManager.Instance.transform.position;

        GameObject poisonCloud = Object.Instantiate(poisonCloudPrefab, playerPosition, Quaternion.identity);
        PoisonCloud cloudScript = poisonCloud.GetComponent<PoisonCloud>();

        if (cloudScript != null)
        {
            cloudScript.Configure(
                (int)card.EffectValue, // Damage per second
                card.ActiveTime,                   // Duration
                0.3f,                              // Poison chance (30%)
                5f,                                // Poison duration
                10                                 // Poison damage per second
            );
        }

        Debug.Log($"Poison Cloud activated: {card.ActiveTime}s, {card.EffectValue} damage per second.");
    }

    public void Disable()
    {
        Debug.Log("Poison Cloud does not require a disable phase.");
    }


}
