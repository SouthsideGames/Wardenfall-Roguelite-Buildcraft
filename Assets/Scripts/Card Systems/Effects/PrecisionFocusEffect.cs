using System.Collections;
using UnityEngine;

public class PrecisionFocusEffect : ICardEffect
{
    private float critChanceBoost;
    private float critDamageBoost;
    private float duration;

    public PrecisionFocusEffect(CardSO cardSO)
    {
        critChanceBoost = cardSO.EffectValue; 
        critDamageBoost = cardSO.CooldownTime;
        duration = cardSO.ActiveTime;  
    }

    public void Activate(float duration)
    {
        CharacterStats.Instance.BoostStat(Stat.CritChance, critChanceBoost);
        CharacterStats.Instance.BoostStat(Stat.CritDamage, critDamageBoost);

        CharacterManager.Instance.StartCoroutine(RevertAfterDuration());
    }

    public void Disable()
    {
       
        CharacterStats.Instance.RevertBoost(Stat.CritChance);
        CharacterStats.Instance.RevertBoost(Stat.CritDamage);
    }

    private IEnumerator RevertAfterDuration()
    {
        yield return new WaitForSeconds(duration);
        Disable();
    }
}
