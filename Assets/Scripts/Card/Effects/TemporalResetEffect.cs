using UnityEngine;

public class TemporalResetEffect : ICardEffect
{
    public void Activate(float duration)
    {
        InGameCardUI[] allCards = Object.FindObjectsByType<InGameCardUI>(FindObjectsSortMode.None);

        foreach (InGameCardUI card in allCards)
        {
            if (card.IsCurrentCard())
                continue;

            card.ResetCooldown();
        }

    }

    public void Disable()
    {
       
    }

    public void ApplySynergy(float synergyBonus)
    {
        throw new System.NotImplementedException();
    }

}
