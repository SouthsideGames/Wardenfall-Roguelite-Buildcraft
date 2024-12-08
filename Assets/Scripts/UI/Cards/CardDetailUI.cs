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
    [SerializeField] private TextMeshProUGUI cardDescriptionText;
    [SerializeField] private TextMeshProUGUI cardRarityText;
    [SerializeField] private Image cardEffectIcon;

    [Header("Settings")]
    [SerializeField] private GameObject detailPanel;

    private void Awake()
    {
        CloseDetail(); // Ensure it's closed on start
    }

    public void ShowDetail(CardSO card)
    {
        cardIcon.sprite = card.Icon;
        cardNameText.text = card.CardName;
        cardDescriptionText.text = card.Description;
        cardRarityText.text = card.Rarity.ToString();
        cardEffectIcon.sprite = card.EffectIcon;

        detailPanel.SetActive(true);
    }

    public void CloseDetail()
    {
        detailPanel.SetActive(false);
    }
}
