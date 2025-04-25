using UnityEngine;
using System.Collections;

public class HyperstrideEffect : MonoBehaviour, ICardEffect
{
    [SerializeField] private Stat targetStat = Stat.MoveSpeed;

    public void Activate(CharacterManager target, CardSO card)
    {
        if (target == null || card == null)
        {
            return;
        }

        CharacterStats stats = target.stats;
        if (stats != null)
        {
            stats.BoostStat(targetStat, card.effectValue);
            StartCoroutine(RevertAfterDelay(stats, targetStat, card.activeTime));
        }

        Destroy(gameObject, card.activeTime + 0.5f);
    }

    private IEnumerator RevertAfterDelay(CharacterStats stats, Stat stat, float delay)
    {
        yield return new WaitForSeconds(delay);
        stats.RevertBoost(stat);
    }

    public void Deactivate() { }
    public void Tick(float deltaTime) { }
}
