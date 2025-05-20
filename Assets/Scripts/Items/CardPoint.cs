using UnityEngine;
using System;
using SouthsideGames.DailyMissions;

public class CardPoint : Item
{
    [Header("ACTIONS:")]
    public static Action<CardPoint> OnCollected;
    protected override void Collected()
    {
        OnCollected?.Invoke(this);
    }   
}
