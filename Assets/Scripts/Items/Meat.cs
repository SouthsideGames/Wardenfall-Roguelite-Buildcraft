using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using SouthsideGames.DailyMissions;

public class Meat : Item
{
    [Header("ACTIONS:")]
    public static Action<Meat> OnCollected;
    
    protected override void Collected()
    {
        MissionManager.Increment(MissionType.currencyCollected, 1);
        MissionManager.Increment(MissionType.currencyCollected2, 1);
        OnCollected?.Invoke(this);
    }   

}
