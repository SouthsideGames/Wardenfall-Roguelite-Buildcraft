using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DecklistCardUI : MonoBehaviour
{
    [Header("Elements")]
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI cardNameText;
    [SerializeField] private TextMeshProUGUI costText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private Image effectIcon;
    [SerializeField] private Button detailButton;

    private CardSO assignedCard;

    public void Configure(CardSO card)
    {
        assignedCard = card;

        icon.sprite = card.Icon;
        cardNameText.text = card.CardName;
        costText.text = card.Cost.ToString();
        descriptionText.text = card.Description;
        effectIcon.sprite = card.EffectIcon;

        detailButton.onClick.RemoveAllListeners();
        detailButton.onClick.AddListener(() => DeckManager.Instance.ShowCardDetails(card));
    }
}
