using UnityEngine;

public class VoidWardenEffect : MonoBehaviour, ICardEffect
{
    [SerializeField] private GameObject voidWardenPrefab;

    public void Activate(CharacterManager target, CardSO card)
    {
        if (target == null || voidWardenPrefab == null)
        {
            Debug.LogWarning("VoidWardenEffect: Target or prefab is missing.");
            return;
        }

        Vector3 spawnPosition = target.transform.position + Vector3.right * 2f;
        GameObject warden = Instantiate(voidWardenPrefab, spawnPosition, Quaternion.identity);

        VoidWarden logic = warden.GetComponent<VoidWarden>();
        if (logic != null)
        {
            logic.Initialize(card.activeTime);
        }

        Destroy(gameObject, card.activeTime + 0.5f);
    }

    public void Deactivate() { }
    public void Tick(float deltaTime) { }
}
