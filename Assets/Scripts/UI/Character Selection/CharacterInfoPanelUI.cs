using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CharacterInfoPanelUI : MonoBehaviour
{
    [Header("ELEMENTS")]
    [SerializeField] private Image backgroundImage;
    [SerializeField] private Image characterImage;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI rarityText;
    [SerializeField] private TextMeshProUGUI priceText;
    [SerializeField] private GameObject priceContainer;
    [SerializeField] private Transform statsParent;

    [Header("Rarity Backgrounds")]
    [SerializeField] private Sprite commonBackground;
    [SerializeField] private Sprite uncommonBackground;
    [SerializeField] private Sprite rareBackground;
    [SerializeField] private Sprite epicBackground;
    [SerializeField] private Sprite legendaryBackground;

    private Dictionary<CharacterCardRarityType, Sprite> rarityBackgrounds;

    [field: SerializeField] public Button Button {get; private set;}  
    [SerializeField] private Button closeButton;  

    private void Awake()
    {
        // Map rarities to background images
        rarityBackgrounds = new Dictionary<CharacterCardRarityType, Sprite>()
        {
            { CharacterCardRarityType.Common, commonBackground },
            { CharacterCardRarityType.Uncommon, uncommonBackground },
            { CharacterCardRarityType.Rare, rareBackground },
            { CharacterCardRarityType.Epic, epicBackground },
            { CharacterCardRarityType.Legendary, legendaryBackground }
        };
    }

    private void Start()
    {
        // Assign close button functionality
        closeButton.onClick.AddListener(ClosePanel);
    }

   public void ConfigureInfoPanel(CharacterDataSO _characterDataSO, bool unlocked)
    {
        nameText.text = _characterDataSO.Name;
        rarityText.text = _characterDataSO.Rarity.ToString();
        priceText.text = _characterDataSO.PurchasePrice.ToString();
        characterImage.sprite = _characterDataSO.Icon;

        priceContainer.SetActive(!unlocked);

        StatContainerManager.GenerateStatContainers(_characterDataSO.NonNeutralStats, statsParent);

        if (rarityBackgrounds.TryGetValue(_characterDataSO.Rarity, out Sprite bgSprite))
        {
            backgroundImage.sprite = bgSprite;
        }
    }

 public void ShowPanel(CharacterDataSO _characterDataSO, bool unlocked)
    {
        gameObject.SetActive(true); 
        ConfigureInfoPanel(_characterDataSO, unlocked); 

        if (unlocked)
        {
            Button.GetComponentInChildren<TextMeshProUGUI>().text = "Select";
            Button.interactable = true; 
        }
        else
        {
            Button.GetComponentInChildren<TextMeshProUGUI>().text = $"Buy {_characterDataSO.PurchasePrice}";
            Button.interactable = CurrencyManager.Instance.HasEnoughPremiumCurrency(_characterDataSO.PurchasePrice);
        }
    }

    public void ClosePanel()
    {
        // Clear UI elements
        nameText.text = "";
        rarityText.text = "";
        priceText.text = "";

        foreach (Transform child in statsParent)
        {
            Destroy(child.gameObject); // Clear stat containers
        }

        gameObject.SetActive(false); // Hide panel
    }
}
