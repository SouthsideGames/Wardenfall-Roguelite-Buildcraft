using UnityEngine;

public class DeathRayEffect : ICardEffect
{
    private GameObject deathCloudPrefab;
    private CardSO cardSO;

    public DeathRayEffect(GameObject _deathCloudPrefab, CardSO _cardSO)
    {
        deathCloudPrefab = _deathCloudPrefab;
        cardSO = _cardSO;
    }

    public void Activate(float duration)
    {
        Vector2 playerPosition = CharacterManager.Instance.transform.position;

        GameObject deathCloud = Object.Instantiate(deathCloudPrefab, playerPosition, Quaternion.identity);
        DeathRay cloudScript = deathCloud.GetComponent<DeathRay>();
        cloudScript.Configure(duration);

    }

    public void Disable()
    {

    }


}
