using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DecklistCardContainerUI : MonoBehaviour
{
    [Header("ELEMENTS:")]
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI cardNameText;
    [SerializeField] private TextMeshProUGUI costText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private TextMeshProUGUI effectText;

    public void Configure(CardSO _cardSO)
    {
        icon.sprite = _cardSO.Icon;
        cardNameText.text = _cardSO.CardName;
        costText.text = _cardSO.Cost.ToString();
        descriptionText.text = _cardSO.Description;
        effectText.text = $"Type: {_cardSO.EffectType}: {_cardSO.EffectValue}";

    }
}
