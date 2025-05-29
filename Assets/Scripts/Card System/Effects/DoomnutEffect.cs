using UnityEngine;

public class DoomnutEffect : MonoBehaviour, ICardEffect
{
    [SerializeField] private GameObject doomnutPrefab;

    public void Activate(CharacterManager target, CardSO card)
    {
        if (target == null || doomnutPrefab == null)
        {
            Debug.LogWarning("DoomnutEffect: Target or prefab is missing.");
            return;
        }

        Vector3 spawnPos = target.transform.position + Vector3.up * 1.5f;
        GameObject donut = Instantiate(doomnutPrefab, spawnPos, Quaternion.identity);

        Doomnut logic = donut.GetComponent<Doomnut>();
        if (logic != null)
        {
            logic.Initialize(card.activeTime);
        }

        Destroy(gameObject, card.activeTime + 0.5f);
    }

    public void Deactivate() { }
    public void Tick(float deltaTime) { }
}
