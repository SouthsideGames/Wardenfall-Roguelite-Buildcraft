using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using SouthsideGames.DailyMissions;

public class Candy : Item
{
     [Header("ACTIONS:")]
    public static Action<Candy> OnCollected;
    
    protected override void Collected()
    {
        MissionManager.Increment(MissionType.currencyCollected, 1);
        OnCollected?.Invoke(this);
    }   
}
