using UnityEngine;
using SouthsideGames.DailyMissions;
using System;

public class Gem : Item
{
    [Header("ACTIONS:")]
    public static Action<Gem> OnCollected;
    protected override void Collected()
    {
        MissionManager.Increment(MissionType.gemCollected, 1);
        MissionManager.Increment(MissionType.gemCollected2, 1);
        OnCollected?.Invoke(this);
    }   
}
