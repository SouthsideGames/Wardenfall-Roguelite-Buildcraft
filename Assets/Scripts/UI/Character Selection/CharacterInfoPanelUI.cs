using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CharacterInfoPanelUI : MonoBehaviour
{
    [Header("ELEMENTS")]
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI priceText;
    [SerializeField] private GameObject priceContainer;
    [SerializeField] private Transform statsParent;
    [SerializeField] private Image backgroundImage;

    [SerializeField] private Sprite commonImage, uncommonImage, rareImage, epicImage, legendaryImage;

    [field: SerializeField] public Button purchasedButton { get; private set; }

    [Header("VIEWS")]
    [SerializeField] private GameObject statsContainer;
    [SerializeField] private GameObject cardsContainer;

    [Header("CARDS")]
    [SerializeField] private Transform cardsContentRoot;
    [SerializeField] private GameObject cardButtonPrefab;

    [Header("DETAIL PANEL")]
    [SerializeField] private GameObject characterDetailPanel;
    [SerializeField] private Image detailIcon;
    [SerializeField] private TextMeshProUGUI detailName;
    [SerializeField] private TextMeshProUGUI detailDescription;
    [SerializeField] private TextMeshProUGUI detailCost;
    [SerializeField] private TextMeshProUGUI detailRarity;
    [SerializeField] private TextMeshProUGUI detailCooldown;
    [SerializeField] private TextMeshProUGUI detailActiveTime;
    [SerializeField] private TextMeshProUGUI detailEffectValue;
    [SerializeField] private TextMeshProUGUI detailEffectType;
    [SerializeField] private Image detailBackgroundImage;

    [Header("RARITY COLORS")]
    [SerializeField] private Color commonColor;
    [SerializeField] private Color uncommonColor;
    [SerializeField] private Color rareColor;
    [SerializeField] private Color epicColor;
    [SerializeField] private Color legendaryColor;

    private bool showingStats = true;
    private List<CardSO> currentCards = new List<CardSO>();

    public void ConfigureInfoPanel(CharacterDataSO _characterDataSO, bool unlocked, List<CardSO> cards)
    {
        // Always default to stats view
        showingStats = true;
        statsContainer.SetActive(true);
        cardsContainer.SetActive(false);

        icon.sprite = _characterDataSO.Icon;
        nameText.text = _characterDataSO.Name;

        if (purchasedButton != null)
        {
            purchasedButton.gameObject.SetActive(!unlocked);
            if (!unlocked)
                priceText.text = _characterDataSO.PurchasePrice.ToString();
        }
        else
            Debug.LogWarning("Price container is not assigned in the inspector.");

        StatContainerManager.GenerateStatContainers(_characterDataSO.NonNeutralStats, statsParent);

        ChangeBackgrounds(_characterDataSO);

        characterDetailPanel.SetActive(false);

        // Store the character's cards
        SetCharacterCards(cards);
    }



    private void ChangeBackgrounds(CharacterDataSO _characterDataSO)
    {

        if (_characterDataSO.Rarity == CharacterRarityType.Common)
            backgroundImage.sprite = commonImage;
        else if (_characterDataSO.Rarity == CharacterRarityType.Uncommon)
            backgroundImage.sprite = uncommonImage;
        else if (_characterDataSO.Rarity == CharacterRarityType.Rare)
            backgroundImage.sprite = rareImage;
        else if (_characterDataSO.Rarity == CharacterRarityType.Epic)
            backgroundImage.sprite = epicImage;
        else if (_characterDataSO.Rarity == CharacterRarityType.Legendary)
            backgroundImage.sprite = legendaryImage;
    }

    public void OnToggleStatsCardsPressed()
    {
        showingStats = !showingStats;

        statsContainer.SetActive(showingStats);
        cardsContainer.SetActive(!showingStats);

        if (!showingStats)
            PopulateCharacterCards();
    }


    public void SetCharacterCards(List<CardSO> cards) => currentCards = cards;


    private void PopulateCharacterCards()
    {
        foreach (Transform child in cardsContentRoot)
            Destroy(child.gameObject);

        foreach (var card in currentCards)
        {
            var newButton = Instantiate(cardButtonPrefab, cardsContentRoot);
            newButton.GetComponent<CharacterCardButtonUI>().Initialize(card, ShowCharacterCardDetail);
        }
    }


    public void ShowCharacterCardDetail(CardSO card)
    {
        characterDetailPanel.SetActive(true);
        detailIcon.sprite = card.icon;
        detailName.text = card.cardName;
        detailDescription.text = card.description;
        detailCost.text = $"Cost: {card.cost}";
        detailRarity.text = $"Rarity: {card.rarity.ToString()}";
        detailCooldown.text = $"Cooldown: {card.cooldown} seconds";
        detailActiveTime.text = $"Active Time: {card.activeTime}";
        detailEffectValue.text = $"Effect Value: {card.effectValue}";
        detailEffectType.text = $"Effect Type: {card.effectType.ToString()}";

        SetDetailBackgroundColor(card.rarity);
    }

    public void CloseCharacterCardDetail()
    {
        characterDetailPanel.SetActive(false);

        detailIcon.sprite = null;
        detailName.text = "";
        detailDescription.text = "";
        detailCost.text = "";
        detailRarity.text = "";
        detailCooldown.text = "";
        detailActiveTime.text = "";
        detailEffectValue.text = "";
        detailEffectType.text = "";

        detailBackgroundImage.color = Color.white;
    }

    public void HideCardDetail() => characterDetailPanel.SetActive(false);

    private void SetDetailBackgroundColor(CardRarity rarity)
    {
        switch (rarity)
        {
            case CardRarity.Common:
                detailBackgroundImage.color = commonColor;
                break;
            case CardRarity.Uncommon:
                detailBackgroundImage.color = uncommonColor;
                break;
            case CardRarity.Rare:
                detailBackgroundImage.color = rareColor;
                break;
            case CardRarity.Epic:
                detailBackgroundImage.color = epicColor;
                break;
            case CardRarity.Legendary:
                detailBackgroundImage.color = legendaryColor;
                break;
            default:
                detailBackgroundImage.color = Color.white;
                break;
        }
    }


}
