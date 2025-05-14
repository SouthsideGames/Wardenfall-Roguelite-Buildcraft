using System.Collections;
using UnityEngine;

public class StatBoostEffect : MonoBehaviour, ICardEffect
{
    [SerializeField] private Stat targetStat;

    public void Activate(CharacterManager target, CardSO card)
    {
        if (target == null || card == null)
        {
            Debug.LogWarning("StatBoostEffect: Missing target or card.");
            return;
        }

        float duration = card.activeTime;
        float value = card.effectValue;

        target.stats.BoostStat(targetStat, value);
        StartCoroutine(RevertAfterTime(target, duration));
        Destroy(gameObject, duration + 0.5f);
    }

    private IEnumerator RevertAfterTime(CharacterManager target, float duration)
    {
        yield return new WaitForSeconds(duration);
        target.stats.RevertBoost(targetStat);
    }

    public void Deactivate() {}
    public void Tick(float deltaTime) {}
}
