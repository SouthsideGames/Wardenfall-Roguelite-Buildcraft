using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ViewCardOptionUI : MonoBehaviour
{
     [Header("UI Elements")]
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private TextMeshProUGUI costText;
    [SerializeField] private Button removeButton;
    [SerializeField] private Button cardClickArea;

    private CardSO card;
    private bool isSelected = false;

    private void Awake()
    {
        if (cardClickArea != null)
            cardClickArea.onClick.AddListener(OnCardClicked);

        removeButton.gameObject.SetActive(false);
    }

    public void Configure(CardSO cardData)
    {
        card = cardData;

        icon.sprite = card.icon;
        nameText.text = card.cardName;
        descriptionText.text = card.description;
        costText.text = $"Cost: {card.cost}";

        isSelected = false;
        removeButton.gameObject.SetActive(false);
    }

    public void OnCardClicked()
    {
        isSelected = !isSelected;
        removeButton.gameObject.SetActive(isSelected);
    }

    public void Deselect()
    {
        isSelected = false;
        removeButton.gameObject.SetActive(false);
    }

    public void RemoveCard()
    {
        if (card != null)
        {
            CharacterManager.Instance.cards.RemoveCard(card);
        }
    }
}
