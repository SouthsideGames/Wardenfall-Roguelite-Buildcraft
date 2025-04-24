using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TraitSuppressionEffect : MonoBehaviour, ICardEffect
{
    private float duration;

    public void Activate(CharacterManager target, CardSO card)
    {
        duration = card.activeTime;

        TraitManager.Instance.DisableAllTraitsTemporarily();

        StartCoroutine(RestoreTraitsAfterDelay());
        Destroy(gameObject, duration + 0.5f);
    }

    private IEnumerator RestoreTraitsAfterDelay()
    {
        yield return new WaitForSeconds(duration);

        TraitManager.Instance.RestoreAllSuppressedTraits();
    }

    public void Deactivate() { }
    public void Tick(float deltaTime) { }
}
