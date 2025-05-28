using UnityEngine;

public class SporePodEffect : MonoBehaviour, ICardEffect
{
    [SerializeField] private GameObject sporePodPrefab;

    public void Activate(CharacterManager target, CardSO card)
    {
        if (target == null || sporePodPrefab == null)
        {
            Debug.LogWarning("SporePodEffect: Target or prefab is missing.");
            return;
        }

        Vector3 spawnPosition = target.transform.position;
        GameObject pod = Instantiate(sporePodPrefab, spawnPosition, Quaternion.identity);

        SporePod podLogic = pod.GetComponent<SporePod>();
        if (podLogic != null)
        {
            podLogic.Initialize(card.activeTime);
        }

        Destroy(gameObject, card.activeTime + 0.5f);
    }

    public void Deactivate() { }
    public void Tick(float deltaTime) { }
}
