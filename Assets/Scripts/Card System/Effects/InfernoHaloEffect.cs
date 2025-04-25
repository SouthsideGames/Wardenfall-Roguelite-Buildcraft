using System.Collections;
using UnityEngine;

public class InfernoHaloEffect : MonoBehaviour, ICardEffect
{
    [SerializeField] private GameObject fireRingPrefab;

    public void Activate(CharacterManager target, CardSO card)
    {
        if (target == null || fireRingPrefab == null)
        {
            Debug.LogWarning("InfernoHaloEffect: Target or prefab is missing.");
            return;
        }

        GameObject fireRing = Instantiate(fireRingPrefab, target.transform.position, Quaternion.identity);
        fireRing.transform.SetParent(target.transform);

        FireRingZone zone = fireRing.GetComponent<FireRingZone>();
        if (zone != null)
        {
            zone.Initialize(card.effectValue, card.activeTime);
        }

        Destroy(gameObject, card.activeTime + 0.5f);
    }

    public void Deactivate() { }
    public void Tick(float deltaTime) { }
}