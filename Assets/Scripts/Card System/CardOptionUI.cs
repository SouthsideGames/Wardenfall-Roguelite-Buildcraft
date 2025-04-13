using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardOptionUI : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private TextMeshProUGUI costText;
    [SerializeField] private Button selectButton;

    private CardSO card;
    private System.Action onSelected;

    public void Configure(CardSO cardData, System.Action selectCallback)
    {
        card = cardData;
        onSelected = selectCallback;

        icon.sprite = card.icon;
        nameText.text = card.cardName;
        descriptionText.text = card.description;
        costText.text = $"Cost: {card.cost}";

        selectButton.onClick.RemoveAllListeners();
        selectButton.onClick.AddListener(() => onSelected?.Invoke());
    }
}