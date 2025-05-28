using UnityEngine;

public class ObsidianGolemEffect : MonoBehaviour, ICardEffect
{
    [SerializeField] private GameObject obsidianGolemPrefab;

    public void Activate(CharacterManager target, CardSO card)
    {
        if (target == null || obsidianGolemPrefab == null)
        {
            Debug.LogWarning("ObsidianGolemEffect: Target or prefab is missing.");
            return;
        }

        Vector3 spawnPosition = target.transform.position + Vector3.right * 2f;
        GameObject golem = Instantiate(obsidianGolemPrefab, spawnPosition, Quaternion.identity);

        ObsidianGolem logic = golem.GetComponent<ObsidianGolem>();
        if (logic != null)
        {
            logic.Initialize(card.activeTime);
        }

        Destroy(gameObject, card.activeTime + 0.5f);
    }

    public void Deactivate() { }
    public void Tick(float deltaTime) { }
}
