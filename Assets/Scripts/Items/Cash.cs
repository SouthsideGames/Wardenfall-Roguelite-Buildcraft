using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using SouthsideGames.DailyMissions;

public class Cash : Item
{
    [Header("ACTIONS:")]
    public static Action<Cash> onCollected;
    
    protected override void Collected()
    {
        MissionManager.Increment(MissionType.premiumCurrencyCollected, 1);
        onCollected?.Invoke(this);
    }   
}
