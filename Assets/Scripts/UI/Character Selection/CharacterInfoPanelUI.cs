using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CharacterInfoPanelUI : MonoBehaviour
{
    [Header("ELEMENTS")]
    [SerializeField] private Image backgroundImage;
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

    public void ConfigureInfoPanel(CharacterDataSO _characterDataSO, bool unlocked)
    {
        nameText.text = _characterDataSO.Name;
        rarityText.text = _characterDataSO.Rarity.ToString();
        priceText.text = _characterDataSO.PurchasePrice.ToString(); 



        priceContainer.SetActive(!unlocked);

        StatContainerManager.GenerateStatContainers(_characterDataSO.NonNeutralStats, statsParent);

        // Set the background based on rarity
        if (rarityBackgrounds.TryGetValue(_characterDataSO.Rarity, out Sprite bgSprite))
        {
            backgroundImage.sprite = bgSprite;
        }
        else
        {
            Debug.LogWarning($"No background set for rarity {_characterDataSO.Rarity}");
        }

    }
}
