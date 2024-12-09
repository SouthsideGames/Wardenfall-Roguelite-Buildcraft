using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardDetailUI : MonoBehaviour
{
    [Header("ELEMENTS:")]
    [SerializeField] private Image cardIcon;
    [SerializeField] private Image backgroundImage;
    [SerializeField] private Image effectAreaImage;
    [SerializeField] private TextMeshProUGUI cardNameText;
    [SerializeField] private TextMeshProUGUI cardEffectName;
    [SerializeField] private TextMeshProUGUI cardEffectDescriptionText;
    [SerializeField] private TextMeshProUGUI cardRarityText;
    [SerializeField] private Image cardEffectIcon;
    [SerializeField] private Transform rarityStarParent;
    [SerializeField] private GameObject cardStarPrefab;
    [SerializeField] private GameObject cardPlatinumStarPrefab;

    [Header("SETTINGS:")]
    [SerializeField] private PaletteSO palette; 

    public void ShowDetail(CardSO card)
    {
        cardIcon.sprite = card.Icon;
        cardNameText.text = card.CardName;
        cardEffectName.text = card.EffectName;
        cardEffectDescriptionText.text = card.Description;
        cardRarityText.text = card.Rarity.ToString();
        cardEffectIcon.sprite = card.EffectIcon;

        UpdateBackgroundColor(card.Rarity);
        UpdateRarityStars(card.Rarity);
    }

     private void UpdateBackgroundColor(CardRarityType rarity)
    {
        int rarityIndex = (int)rarity;

        if (palette != null && palette.cardDetailColors.Length > rarityIndex)
        {
            backgroundImage.color = palette.cardDetailColors[rarityIndex];
            effectAreaImage.color = palette.cardDetailColors[rarityIndex];

        }
    }

    private void UpdateRarityStars(CardRarityType rarity)
    {
        // Clear existing stars
        foreach (Transform child in rarityStarParent)
        {
            Destroy(child.gameObject);
        }

        int starCount = GetStarCount(rarity);
        GameObject starPrefab = rarity == CardRarityType.Exalted ? cardPlatinumStarPrefab : cardStarPrefab;

        // Spawn stars
        for (int i = 0; i < starCount; i++)
        {
            Instantiate(starPrefab, rarityStarParent);
        }
    }

    private int GetStarCount(CardRarityType rarity)
    {
        switch (rarity)
        {
            case CardRarityType.Common: return 1;
            case CardRarityType.Uncommon: return 2;
            case CardRarityType.Rare: return 3;
            case CardRarityType.Epic: return 4;
            case CardRarityType.Legendary: return 5;
            case CardRarityType.Mythic: return 6;
            case CardRarityType.Exalted: return 6; // Exalted uses platinum stars
            default: return 0;
        }
    }


}
