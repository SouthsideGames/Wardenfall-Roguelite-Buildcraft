using UnityEngine;
using System.Collections;

public class HyperstrideEffect : MonoBehaviour, ICardEffect
{
    [SerializeField] private Stat targetStat = Stat.MoveSpeed;

    public void Activate(CharacterManager target, CardSO card)
    {
        if (target == null || card == null)
        {
            Debug.LogWarning("HyperstrideEffect: Missing target or card.");
            return;
        }

        CharacterStats stats = target.stats;
        if (stats != null)
        {
            stats.BoostStat(targetStat, card.effectValue);
            Debug.Log($"Hyperstride activated: +{card.effectValue} to {targetStat} for {card.activeTime} seconds.");
            StartCoroutine(RevertAfterDelay(stats, targetStat, card.activeTime));
        }

        Destroy(gameObject, card.activeTime + 0.5f);
    }

    private IEnumerator RevertAfterDelay(CharacterStats stats, Stat stat, float delay)
    {
        yield return new WaitForSeconds(delay);
        stats.RevertBoost(stat);
        Debug.Log("Hyperstride ended: movement speed reverted.");
    }

    public void Deactivate() { }
    public void Tick(float deltaTime) { }
}
