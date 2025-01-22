using UnityEngine;

public class BerserkerRageEffect : ICardEffect
{
    private readonly float attackBoostValue;
    private readonly float duration;

    public BerserkerRageEffect(CardSO _card)
    {
        attackBoostValue = _card.EffectValue;
        duration = _card.ActiveTime;
    }

    public void Activate(float activeDuration)
    {
        CharacterStats.Instance.BoostStat(Stat.Attack, attackBoostValue);

        Debug.Log($"Berserker Rage activated! Attack increased by {attackBoostValue} for {duration} seconds.");
        CharacterManager.Instance.StartCoroutine(DeactivateAfterDuration());
    }

    public void Disable()
    {
        CharacterStats.Instance.RevertBoost(Stat.Attack);
        Debug.Log("Berserker Rage ended. Attack boost reverted.");
    }

    private System.Collections.IEnumerator DeactivateAfterDuration()
    {
        yield return new WaitForSeconds(duration);
        Disable();
    }
}
