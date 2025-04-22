using UnityEngine;

public class VoidEffect : MonoBehaviour, ICardEffect
{
    [SerializeField] private GameObject voidPrefab;

    public void Activate(CharacterManager target, CardSO card)
    {
        if (voidPrefab == null)
            return;

        Vector2 spawnPosition = target.transform.position;
        GameObject _void = Instantiate(voidPrefab, spawnPosition, Quaternion.identity);
        
        if (_void.TryGetComponent<Void>(out var hole))
        {
            hole.Initialize(card.effectValue, card.activeTime);
        }

        Destroy(gameObject);
    }

    public void Deactivate() {}
    public void Tick(float deltaTime) {}
}
