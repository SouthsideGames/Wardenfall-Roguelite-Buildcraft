using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class AdrenalineSurgeEffect : ICardEffect
{
    private readonly float speedBoostValue;
    private readonly float duration;
    
    public AdrenalineSurgeEffect(CardSO _cardSO)
    {
        speedBoostValue = _cardSO.EffectValue;
        duration = _cardSO.ActiveTime;
    }

    public void Activate(float activeDuration)
    {
        CharacterStats.Instance.BoostStat(Stat.MoveSpeed, speedBoostValue);
        CoroutineRunner.Instance.RunPooled(DeactivateAfterDuration());  
    }

    public void Disable()
    {
        CharacterStats.Instance.RevertBoost(Stat.MoveSpeed);
    }

    private IEnumerator DeactivateAfterDuration()
    {
        yield return new WaitForSeconds(duration);
        Disable();
    }
}
