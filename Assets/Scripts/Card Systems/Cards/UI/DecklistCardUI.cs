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
    [SerializeField] private Image effectIcon;

    public void Configure(CardSO card)
    {

        icon.sprite = card.Icon;
        cardNameText.text = card.CardName;
        costText.text = card.Cost.ToString();
        effectIcon.sprite = card.EffectIcon;
    }
}
