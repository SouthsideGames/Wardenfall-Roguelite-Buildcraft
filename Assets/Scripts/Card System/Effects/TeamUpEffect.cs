using UnityEngine;

public class TeamUpEffect : MonoBehaviour, ICardEffect
{
    [SerializeField] private GameObject teamUpPrefab;

    public void Activate(CharacterManager target, CardSO card)
    {
        if (target == null || teamUpPrefab == null)
        {
            Debug.LogWarning("TeamUpEffect: Target or prefab is missing.");
            return;
        }

        Vector3 spawnPosition = target.transform.position + Vector3.up * 1f;
        GameObject familiar = Instantiate(teamUpPrefab, spawnPosition, Quaternion.identity);

        TeamUp logic = familiar.GetComponent<TeamUp>();
        if (logic != null)
        {
            logic.Initialize(target.transform, card.activeTime);
        }

        Destroy(gameObject, card.activeTime + 0.5f);
    }

    public void Deactivate() { }
    public void Tick(float deltaTime) { }
}
