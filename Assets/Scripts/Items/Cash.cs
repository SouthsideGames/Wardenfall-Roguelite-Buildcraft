using UnityEngine;
using System;
using SouthsideGames.DailyMissions;

public class Cash : Item
{
    [Header("ACTIONS:")]
    public static Action<Cash> OnCollected;
    protected override void Collected()
    {
        MissionManager.Increment(MissionType.currencyCollected, 1);
        OnCollected?.Invoke(this);
    }   
}
