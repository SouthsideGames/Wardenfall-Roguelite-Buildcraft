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

        if (rewardType == ItemRewardType.Card)
        {
            CardSO[] allCards = Resources.LoadAll<CardSO>("Data/Cards");
            List<CardSO> uncollectedCards = allCards.Where(card => !card.IsCollected).ToList();

            if (uncollectedCards.Count > 0)
            {
                CardSO selectedCard = uncollectedCards[UnityEngine.Random.Range(0, uncollectedCards.Count)];
                iconRenderer.sprite = selectedCard.Icon;
                amountText.text = selectedCard.CardName;
            }
            else
            {
                Debug.LogWarning("No uncollected cards available.");
                amountText.text = "No Cards";
            }
        }
        else
        {
            if (amountText != null)
            {
                amountText.gameObject.SetActive(true);
                amountText.text = amount.ToString();
            }
        }
    }

    protected override void Collected()
    {
        switch (rewardType)
        {
            case ItemRewardType.Cash:
                CurrencyManager.Instance.AdjustPremiumCurrency(amount);
                break;
            case ItemRewardType.Gem:
                CurrencyManager.Instance.AdjustGemCurrency(amount);
                break;
            case ItemRewardType.Card:
                CardSO[] allCards = Resources.LoadAll<CardSO>("Data/Cards");
                List<CardSO> uncollectedCards = allCards.Where(card => !card.IsCollected).ToList();

                if (uncollectedCards.Count > 0)
                {
                    CardSO selectedCard = uncollectedCards[UnityEngine.Random.Range(0, uncollectedCards.Count)];

                    selectedCard.Collected();
                    SaveManager.Save(selectedCard, selectedCard.ID, true);
                }
                break;
        }

        OnCollected?.Invoke(this);
        Destroy(gameObject);
    }

    public void Collect() => Collected();

    
}
