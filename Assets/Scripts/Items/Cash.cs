using UnityEngine;
using System;
using SouthsideGames.DailyMissions;

public class Cash : Item
{
    [Header("ACTIONS:")]
    public static Action<Cash> OnCollected;

    [Header("SETTINGS:")]
    [SerializeField] private AudioClip collectSFX;

    protected override void Collected()
    {
        AudioManager.Instance.PlaySFX(collectSFX);
        MissionManager.Increment(MissionType.premiumCurrencyCollected, 1);
        MissionManager.Increment(MissionType.premiumCurrencyCollected2, 1);
        OnCollected?.Invoke(this);
    }   
}
