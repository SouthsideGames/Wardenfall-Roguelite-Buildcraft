using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardShopUI : MonoBehaviour
{
     [Header("UI Elements")]
    [SerializeField] private Image cardIcon;
    [SerializeField] private Image effectIcon;
    [SerializeField] private TextMeshProUGUI cardNameText;
    [SerializeField] private TextMeshProUGUI costText;
    [SerializeField] private TextMeshProUGUI purchasePriceText;
    [SerializeField] private Button purchaseButton;

    private CardSO card;

    public void Configure(CardSO card)
    {
        this.card = card;

        cardIcon.sprite = card.Icon;
        effectIcon.sprite = card.EffectIcon;
        cardNameText.text = card.CardName;
        costText.text = card.Cost.ToString();
        purchasePriceText.text = $"{card.PurchasePrice} Gems";

        purchaseButton.onClick.RemoveAllListeners();
        purchaseButton.onClick.AddListener(PurchaseCard);

        UpdateUI();
    }

    public void UpdateUI()
    {
        purchaseButton.interactable = CurrencyManager.Instance.HasEnoughGem(card.PurchasePrice);
    }

    private void PurchaseCard()
    {
        if (CurrencyManager.Instance.HasEnoughGem(card.PurchasePrice))
        {
            CurrencyManager.Instance.UseGemCurrency(card.PurchasePrice);
            card.Collected();
            Debug.Log($"{card.CardName} purchased for {card.PurchasePrice} Gems!");
            UpdateUI();
        }
        else
        {
            Debug.Log("Not enough Gems to purchase this card!");
        }
    }
}
