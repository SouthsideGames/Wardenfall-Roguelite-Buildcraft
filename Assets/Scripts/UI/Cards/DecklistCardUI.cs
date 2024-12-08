using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DecklistCardUI : MonoBehaviour
{
    [Header("ELEMENTS:")]
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI cardNameText;
    [SerializeField] private TextMeshProUGUI costText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private Image effectIcon;

    public void Configure(CardSO _cardSO)
    {
        icon.sprite = _cardSO.Icon;
        cardNameText.text = _cardSO.CardName;
        costText.text = _cardSO.Cost.ToString();
        //descriptionText.text = _cardSO.Description;
        effectIcon.sprite = _cardSO.EffectIcon;

    }
}
