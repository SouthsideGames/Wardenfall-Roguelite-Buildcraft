using UnityEngine;

public class DetonabotEffect : MonoBehaviour, ICardEffect
{
    [SerializeField] private GameObject detonabotPrefab;

    public void Activate(CharacterManager target, CardSO card)
    {
        if (target == null || detonabotPrefab == null)
        {
            Debug.LogWarning("DetonabotEffect: Missing target or prefab.");
            return;
        }

        Vector3 spawnPos = target.transform.position;
        GameObject bot = Instantiate(detonabotPrefab, spawnPos, Quaternion.identity);
        DetonabotAI logic = bot.GetComponent<DetonabotAI>();

        if (logic != null)
        {
            logic.Initialize(card);
        }
    }

    public void Deactivate()
    {
    }

    public void Tick(float deltaTime)
    {
    }
}
