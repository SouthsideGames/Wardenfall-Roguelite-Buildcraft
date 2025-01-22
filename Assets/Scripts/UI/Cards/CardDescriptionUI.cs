using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardDescriptionUI : MonoBehaviour
{
    [Header("ELEMENTS")]
    [SerializeField] private Image cardImage;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI rarityText;
    [SerializeField] private TextMeshProUGUI activeTimeText;
    [SerializeField] private TextMeshProUGUI cooldownText;
    [SerializeField] private TextMeshProUGUI descriptionText;

    [Header("BACKGROUND ELEMENTS:")]
    [SerializeField] private Image background;
    [SerializeField] private Sprite commonBackgroundImage;
    [SerializeField] private Sprite uncommonBackgroundImage;
    [SerializeField] private Sprite rareBackgroundImage;
    [SerializeField] private Sprite epicBackgroundImage;
    [SerializeField] private Sprite legendaryBackgroundImage;
    [SerializeField] private Sprite mythicBackgroundImage;
    [SerializeField] private Sprite exaltedBackgroundImage;

    private void OnEnable()
    {
        CardHandlerUI.OnButtonPressed += Configure; 
    }

    private void OnDisable()
    {
        CardHandlerUI.OnButtonPressed -= Configure;
    }


    public void Configure(CardSO _card)
    {
        if (_card == null)
        {
            Debug.LogError("CardSO is null. Cannot configure card UI.");
            return;
        }

        cardImage.sprite = _card.Icon;
        nameText.text = _card.CardName;
        rarityText.text = _card.Rarity.ToString();
        activeTimeText.text = $"Active Time: {_card.ActiveTime}s";
        cooldownText.text = $"Cooldown: {_card.CooldownTime}s";
        descriptionText.text = _card.Description;

        ChangeBackground(_card);    
    }

    private void ChangeBackground(CardSO _cardSO)
    {
        if (_cardSO.Rarity == CardRarityType.Common)
            background.sprite = commonBackgroundImage;
        else if (_cardSO.Rarity == CardRarityType.Uncommon)
            background.sprite = uncommonBackgroundImage;
        else if (_cardSO.Rarity == CardRarityType.Rare)
            background.sprite = rareBackgroundImage;
        else if (_cardSO.Rarity == CardRarityType.Epic)
            background.sprite = epicBackgroundImage;
        else if (_cardSO.Rarity == CardRarityType.Legendary)
            background.sprite = legendaryBackgroundImage;
        else if (_cardSO.Rarity == CardRarityType.Mythic)
            background.sprite = mythicBackgroundImage;
        else if (_cardSO.Rarity == CardRarityType.Exalted)
            background.sprite = exaltedBackgroundImage;
    }
}
