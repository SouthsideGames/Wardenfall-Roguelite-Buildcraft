using UnityEngine;
using TMPro;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;
using SouthsideGames.SaveManager;

public class Reward : Item
{
     [SerializeField] private TextMeshProUGUI amountText;
    [SerializeField] private Image iconRenderer;

    [Header("ACTIONS:")]
    public static Action<Reward> OnCollected;

    private int amount;
    [SerializeField] private ItemRewardType rewardType;

    public void SetAmount(int amount, ItemRewardType rewardType)
    {
        this.amount = amount;
        this.rewardType = rewardType;

    }

    protected override void Collected()
    {
        switch (rewardType)
        {
            case ItemRewardType.Cash:
                CurrencyManager.Instance.AdjustPremiumCurrency(amount);
                break;
            case ItemRewardType.CardPoints:
                CurrencyManager.Instance.AdjustCardCurrency(amount);
                break;
            case ItemRewardType.UnlockTickets:
                ProgressionManager.Instance.AdjustUnlockCurrency(amount);
                break;
        }

        OnCollected?.Invoke(this);
        Destroy(gameObject);
    }

    public void Collect() => Collected();

    
}
