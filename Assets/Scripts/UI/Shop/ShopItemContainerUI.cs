using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopItemContainerUI : MonoBehaviour
{
     [Header("ACTIONS:")]
      public static Action<ShopItemContainerUI, int> onPurchased;
    public WeaponDataSO WeaponData {get; private set;}
    public ObjectDataSO ObjectData {get; private set;}

     [Header("ELEMENTS:")]
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI priceText;

    [Header("LEVEL COLORS:")]
    [SerializeField] private Image[] containerImages;
    private int weaponLevel;

    [Header("STATS:")]
    [SerializeField] private Transform statContainerParent;

    [Header("LOCK ELEMENTS:")]
    [SerializeField] private Image lockButton;
    [SerializeField] private Sprite lockedSprite, unlockedSprite;
    public bool isLocked {get; private set;}  

    [SerializeField] private Button purchaseButton;  

    private void Awake() 
    {
       CurrencyManager.onCurrencyUpdate += CurrencyUpdatedCallback;
    }
    
    private void OnDestroy() 
    {
       CurrencyManager.onCurrencyUpdate -= CurrencyUpdatedCallback;
    }

    public void Configure(WeaponDataSO _weaponData, int _level)
    {
        weaponLevel = _level;
        WeaponData = _weaponData;
        
        icon.sprite = _weaponData.Icon;
        nameText.text = _weaponData.Name +  "\n (lvl " + (_level + 1) + ")";

        int weaponPrice = WeaponStatCalculator.GetPurchasePrice(_weaponData, _level);
        priceText.text = weaponPrice.ToString();

        Color imageColor = ColorHolder.GetColor(_level);
        nameText.color = imageColor;  

        foreach(Image image in containerImages)
          image.color = imageColor;     


        Dictionary<Stat, float> calculatedStats = WeaponStatCalculator.GetStats(_weaponData, _level);
        ConfigureStatContainers(calculatedStats);

        purchaseButton.onClick.AddListener(Purchase);
        purchaseButton.interactable = CurrencyManager.Instance.HasEnoughCurrency(weaponPrice);

    }

    public void Configure(ObjectDataSO _objectData)
    {
        ObjectData = _objectData;

        icon.sprite = _objectData.Icon;
        nameText.text = _objectData.Name;
        priceText.text = _objectData.Price.ToString();


        Color imageColor = ColorHolder.GetColor(_objectData.Rarity);
        nameText.color = imageColor;  

        foreach(Image image in containerImages)
          image.color = imageColor;     

        ConfigureStatContainers(_objectData.BaseStats);

        purchaseButton.onClick.AddListener(Purchase);
        purchaseButton.interactable = CurrencyManager.Instance.HasEnoughCurrency(_objectData.Price);

    }


    private void ConfigureStatContainers(Dictionary<Stat, float> _stats)
    {
        statContainerParent.Clear();
        StatContainerManager.GenerateStatContainers(_stats, statContainerParent);
    }

    public void LockButtonCallback()
    {
       isLocked = !isLocked;
       UpdateLockVisuals();
    }

  private void UpdateLockVisuals() => lockButton.sprite = isLocked ? lockedSprite : unlockedSprite;

  private void Purchase()
  {
     onPurchased?.Invoke(this, weaponLevel);
  }

  private void CurrencyUpdatedCallback()
  {
     int itemPrice;

      if(WeaponData != null)
         itemPrice = WeaponStatCalculator.GetPurchasePrice(WeaponData, weaponLevel);
      else
         itemPrice = ObjectData.Price;

      purchaseButton.interactable = CurrencyManager.Instance.HasEnoughCurrency(itemPrice);   
  }

} 