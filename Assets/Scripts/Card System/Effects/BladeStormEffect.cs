using UnityEngine;

public class BladeStormEffect : MonoBehaviour, ICardEffect
{
    [SerializeField] private GameObject bladePrefab;

    public void Activate(CharacterManager target, CardSO card)
    {
        if (bladePrefab == null || target == null)
        {
            Debug.LogWarning("BladeStormEffect missing prefab or target.");
            return;
        }

        GameObject zone = Instantiate(bladePrefab, target.transform.position, Quaternion.identity);
        zone.transform.SetParent(target.transform); // follow the player

        BladeZone zoneScript = zone.GetComponent<BladeZone>();
        if (zoneScript != null)
        {
            zoneScript.Initialize(card.effectValue, card.activeTime);
        }

        Destroy(gameObject);
    }

    public void Deactivate() { }
    public void Tick(float deltaTime) { }
}
