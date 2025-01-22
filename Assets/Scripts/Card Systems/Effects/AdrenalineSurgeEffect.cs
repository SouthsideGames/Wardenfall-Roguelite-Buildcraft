using UnityEngine;

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

        Debug.Log($"Adrenaline Surge activated! Movement speed increased by {speedBoostValue} for {duration} seconds.");
        CharacterManager.Instance.StartCoroutine(DeactivateAfterDuration());
    }

    public void Disable()
    {
        CharacterStats.Instance.RevertBoost(Stat.MoveSpeed);
        Debug.Log("Adrenaline Surge ended. Movement speed boost reverted.");
    }

    private System.Collections.IEnumerator DeactivateAfterDuration()
    {
        yield return new WaitForSeconds(duration);
        Disable();
    }
}
