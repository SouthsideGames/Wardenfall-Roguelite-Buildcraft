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

    [field: SerializeField] public Button Button { get; private set; }

    public void ConfigureInfoPanel(CharacterDataSO _characterDataSO, bool unlocked)
    {
        
        icon.sprite = _characterDataSO.Icon;
        nameText.text = _characterDataSO.Name;
        priceText.text = _characterDataSO.PurchasePrice.ToString();
        priceContainer.SetActive(!unlocked);

        StatContainerManager.GenerateStatContainers(_characterDataSO.NonNeutralStats, statsParent);

        ChangeBackgrounds(_characterDataSO);

    }

    private void ChangeBackgrounds(CharacterDataSO _characterDataSO)
    {

        if (_characterDataSO.Rarity == CharacterCardRarityType.Common)
            backgroundImage.sprite = commonImage;
        else if (_characterDataSO.Rarity == CharacterCardRarityType.Uncommon)
            backgroundImage.sprite = uncommonImage;
        else if (_characterDataSO.Rarity == CharacterCardRarityType.Rare)
            backgroundImage.sprite = rareImage;
        else if (_characterDataSO.Rarity == CharacterCardRarityType.Epic)
            backgroundImage.sprite = epicImage;
        else if (_characterDataSO.Rarity == CharacterCardRarityType.Legendary)
            backgroundImage.sprite = legendaryImage;
    }
}
