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

        Debug.Log($"Precision Focus activated: +{critChanceBoost}% Crit Chance, +{critDamageBoost}% Crit Damage for {duration} seconds.");

        CharacterManager.Instance.StartCoroutine(RevertAfterDuration());
    }

    public void Disable()
    {
       
        CharacterStats.Instance.RevertBoost(Stat.CritChance);
        CharacterStats.Instance.RevertBoost(Stat.CritDamage);

        Debug.Log("Precision Focus disabled: Critical chance and damage boosts removed.");
    }

    private IEnumerator RevertAfterDuration()
    {
        yield return new WaitForSeconds(duration);
        Disable();
    }
}
