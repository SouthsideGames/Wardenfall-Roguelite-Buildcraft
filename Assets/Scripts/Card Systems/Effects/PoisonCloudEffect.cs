using UnityEngine;

public class PoisonCloudEffect : ICardEffect
{
    private GameObject poisonCloudPrefab;
    private CardSO card;

    public PoisonCloudEffect(GameObject _poisonCloudPrefab, CardSO _card)
    {
        poisonCloudPrefab = _poisonCloudPrefab;
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
                (int)card.EffectValue,
                card.ActiveTime,             
                0.3f,                          
                5f,                            
                10                            
            );
        }
    }

    public void Disable()
    {

    }


}
