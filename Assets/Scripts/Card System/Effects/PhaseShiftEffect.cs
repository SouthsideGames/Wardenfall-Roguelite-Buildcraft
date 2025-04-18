using System.Collections;
using UnityEngine;  

public class PhaseShiftEffect : MonoBehaviour, ICardEffect
{
    public void Activate(CharacterManager target, CardSO card)
    {
        if (target == null)
            return;

        // Apply invincibility
        target.health.SetInvincible(true);

        target.StartCoroutine(RevertAfterDuration(target, card.activeTime));

        Destroy(gameObject, card.activeTime + 0.5f);
    }

    private IEnumerator RevertAfterDuration(CharacterManager target, float delay)
    {
        yield return new WaitForSeconds(delay);
        target.health.SetInvincible(false);
    }

    public void Deactivate() { }
    public void Tick(float deltaTime) { }
}
