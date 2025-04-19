using UnityEngine;
using System;
using SouthsideGames.DailyMissions;

public class Cash : Item
{
    [Header("ACTIONS:")]
    public static Action<Cash> OnCollected;
    protected override void Collected()
    {
        MissionManager.Increment(MissionType.premiumCurrencyCollected, 1);
        MissionManager.Increment(MissionType.premiumCurrencyCollected2, 1);
        OnCollected?.Invoke(this);
    }   
}
