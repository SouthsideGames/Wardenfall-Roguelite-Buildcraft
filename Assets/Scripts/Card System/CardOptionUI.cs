using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardOptionUI : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI costText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private Button selectButton;

    private CardSO card;
    private System.Action onSelected;

    public void SetCard(CardSO cardData, System.Action onClick)
    {
        card = cardData;
        onSelected = onClick;

        iconImage.sprite = card.icon;
        nameText.text = card.cardName;
        descriptionText.text = card.description;
        costText.text = $"Cost: {card.cost}";

        selectButton.onClick.RemoveAllListeners();
        selectButton.onClick.AddListener(() => onSelected?.Invoke());
    }
}