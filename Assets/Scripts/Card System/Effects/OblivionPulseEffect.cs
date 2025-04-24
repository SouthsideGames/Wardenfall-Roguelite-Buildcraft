using UnityEngine;

public class OblivionPulseEffect : MonoBehaviour, ICardEffect
{
    [SerializeField] private int targetsToTap = 5;
    [SerializeField] private float timePerTarget = 0.8f;

    public void Activate(CharacterManager target, CardSO card)
    {
        if (QTEManager.Instance == null)
        {
            Debug.LogWarning("OblivionPulseEffect: QTEManager not found.");
            return;
        }

        QTEManager.Instance.StartQTE(
            targetsToTap,
            timePerTarget,
            onComplete: TriggerGlobalKill,
            onFail: () => Debug.Log("Oblivion Pulse QTE failed."));

        Destroy(gameObject, card.activeTime + 0.5f);
    }

    private void TriggerGlobalKill()
    {
        foreach (Enemy enemy in FindObjectsByType<Enemy>(FindObjectsSortMode.None))
        {
            if (enemy == null) continue; // Skip if enemy is destroyed

            enemy.TakeDamage(int.MaxValue); // Instant kill
        }

        Debug.Log("Oblivion Pulse succeeded: All enemies eliminated.");
    }

    public void Deactivate() { }
    public void Tick(float deltaTime) { }
}
