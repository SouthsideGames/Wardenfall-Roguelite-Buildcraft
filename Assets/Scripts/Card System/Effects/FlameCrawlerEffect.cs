using System.Collections;
using UnityEngine;

public class FlameCrawlerEffect : MonoBehaviour, ICardEffect
{
    [SerializeField] private GameObject flameCrawlerPrefab;

    public void Activate(CharacterManager target, CardSO card)
    {
        if (target == null || flameCrawlerPrefab == null)
        {
            Debug.LogWarning("FlameCrawlerEffect: Target or prefab is missing.");
            return;
        }

        // Spawn the crawler slightly in front of the player
        Vector3 spawnPosition = target.transform.position + target.transform.right * 1.5f;
        Instantiate(flameCrawlerPrefab, spawnPosition, Quaternion.identity);

        Destroy(gameObject, card.activeTime + 0.5f);  // Safe cleanup
    }

    public void Deactivate() { }
    public void Tick(float deltaTime) { }
}
