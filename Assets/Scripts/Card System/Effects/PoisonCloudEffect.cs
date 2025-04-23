using UnityEngine;

public class PoisonCloudEffect : MonoBehaviour, ICardEffect
{
    [SerializeField] private GameObject poisonCloudPrefab;

    public void Activate(CharacterManager target, CardSO card)
    {
        if (poisonCloudPrefab == null || target == null)
        {
            Debug.LogWarning("PoisonCloudEffect missing reference.");
            return;
        }

        Vector2 spawnPosition = target.transform.position;
        GameObject cloud = Instantiate(poisonCloudPrefab, spawnPosition, Quaternion.identity);

        PoisonCloudZone zone = cloud.GetComponent<PoisonCloudZone>();
        if (zone != null)
        {
            zone.Initialize((int)card.effectValue, card.activeTime);
        }

        Destroy(gameObject);
    }

    public void Deactivate() {}
    public void Tick(float deltaTime) {}
}
