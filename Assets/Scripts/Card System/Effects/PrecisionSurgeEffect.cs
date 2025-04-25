using System.Collections;
using UnityEngine;

public class PrecisionSurgeEffect : MonoBehaviour, ICardEffect
{
    [SerializeField] private Stat critDamageStat = Stat.CritDamage;

    public void Activate(CharacterManager target, CardSO card)
    {
        if (target == null || card == null)
            return;

        float boostAmount = card.effectValue;
        target.stats.BoostStat(critDamageStat, boostAmount);

        StartCoroutine(RemoveAfterDuration(card.activeTime, target));
        Destroy(gameObject, card.activeTime + 0.5f);
    }

    private IEnumerator RemoveAfterDuration(float duration, CharacterManager target)
    {
        yield return new WaitForSeconds(duration);
        target.stats.RevertBoost(critDamageStat);
    }

    public void Deactivate() { }
    public void Tick(float deltaTime) { }
}