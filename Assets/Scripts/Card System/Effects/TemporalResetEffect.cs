using UnityEngine;

public class TemporalResetEffect : MonoBehaviour, ICardEffect
{
    public void Activate(CharacterManager target, CardSO card)
    {
        if (target == null)
            return;

        InGameCardUIManager uiManager = FindAnyObjectByType<InGameCardUIManager>();
        if (uiManager != null)
            uiManager.ResetAllCooldowns();

        Destroy(gameObject);
    }

    public void Deactivate() { }
    public void Tick(float deltaTime) { }
}
