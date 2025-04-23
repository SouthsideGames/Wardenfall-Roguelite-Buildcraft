using UnityEngine;

public class TimeRiftEffect : MonoBehaviour, ICardEffect
{
    [SerializeField] private GameObject riftZonePrefab;

    public void Activate(CharacterManager target, CardSO card)
    {
        if (riftZonePrefab == null || target == null)
        {
            Debug.LogWarning("TimeRiftEffect missing reference.");
            return;
        }

        Vector2 spawnPosition = target.transform.position;
        GameObject zone = Instantiate(riftZonePrefab, spawnPosition, Quaternion.identity);

        TimeRiftZone zoneScript = zone.GetComponent<TimeRiftZone>();
        if (zoneScript != null)
        {
            zoneScript.Initialize(card.effectValue, card.activeTime);
        }

        Destroy(gameObject);
    }

    public void Deactivate() { }
    public void Tick(float deltaTime) { }
}
