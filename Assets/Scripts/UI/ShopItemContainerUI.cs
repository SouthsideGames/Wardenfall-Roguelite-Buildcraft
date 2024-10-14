using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopItemContainerUI : MonoBehaviour
{
     [Header("ELEMENTS:")]
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI priceText;

    [Header("LEVEL COLORS:")]
    [SerializeField] private Image[] containerImages;
    [SerializeField] private Outline outline;

    [Header("STATS:")]
    [SerializeField] private Transform statContainerParent;

    [Header("SETTING:")]
    [SerializeField] private float scaleSize = 1.075f;
    [SerializeField] private float animationSpeed = .3f;

    [field: SerializeField] public Button PurchaseButton { get; private set; }    

    public void Configure(WeaponDataSO _weaponData, int _level)
    {
        icon.sprite = _weaponData.Icon;
        nameText.text = _weaponData.Name +  "\n (lvl " + (_level + 1) + ")";
        priceText.text = WeaponStatCalculator.GetPurchasePrice(_weaponData, _level).ToString();

        Color imageColor = ColorHolder.GetColor(_level);
        nameText.color = imageColor;  

        outline.effectColor = ColorHolder.GetOutlineColor(_level);

        foreach(Image image in containerImages)
          image.color = imageColor;     


        Dictionary<Stat, float> calculatedStats = WeaponStatCalculator.GetStats(_weaponData, _level);
        ConfigureStatContainers(calculatedStats);

    }

    public void Configure(ObjectDataSO _objectData)
    {
        icon.sprite = _objectData.Icon;
        nameText.text = _objectData.Name;
        priceText.text = _objectData.Price.ToString();


        Color imageColor = ColorHolder.GetColor(_objectData.Rarity);
        nameText.color = imageColor;  

        outline.effectColor = ColorHolder.GetOutlineColor(_objectData.Rarity);

        foreach(Image image in containerImages)
          image.color = imageColor;     

        ConfigureStatContainers(_objectData.BaseStats);

    }


    private void ConfigureStatContainers(Dictionary<Stat, float> _stats)
    {
        statContainerParent.Clear();
        StatContainerManager.GenerateStatContainers(_stats, statContainerParent);
    }


}
