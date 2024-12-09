using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardDetailUI : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Image cardIcon;
    [SerializeField] private TextMeshProUGUI cardNameText;
    [SerializeField] private TextMeshProUGUI cardEffectName;
    [SerializeField] private TextMeshProUGUI cardEffectDescriptionText;
    [SerializeField] private TextMeshProUGUI cardRarityText;
    [SerializeField] private Image cardEffectIcon;

    public void ShowDetail(CardSO card)
    {
        cardIcon.sprite = card.Icon;
        cardNameText.text = card.CardName;
        cardEffectName.text = card.EffectName;
        cardEffectDescriptionText.text = card.Description;
        cardRarityText.text = card.Rarity.ToString();
        cardEffectIcon.sprite = card.EffectIcon;
    }

}
