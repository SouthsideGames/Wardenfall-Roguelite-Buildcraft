using UnityEngine;
using SouthsideGames.DailyMissions;
using System;

public class Gem : Item
{
    [Header("ACTIONS:")]
    public static Action<Gem> OnCollected;
    protected override void Collected()
    {
        MissionManager.Increment(MissionType.currencyCollected, 1);
        OnCollected?.Invoke(this);
    }   
}
