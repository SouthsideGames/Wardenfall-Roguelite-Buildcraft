using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;
using Random = UnityEngine.Random;  

// TODO: update object purchase code to limit the objects from unlimited to limited
public class ShopManager : MonoBehaviour, IGameStateListener
{
    public static Action onItemPurchased;

    [Header("ELEMENTS:")]
    [SerializeField] private Transform containersParent;
    [SerializeField] private ShopItemContainerUI shopItemButton;
    [SerializeField] private Button rerollButton;
    [SerializeField] private int rerollPrice;
    [SerializeField] private TextMeshProUGUI rerollPriceText;

    [Header("PLAYER COMPONENTS:")]
    [SerializeField] private CharacterWeapon characterWeapon;
    [SerializeField] private CharacterObjects characterObject;

    private void Awake() 
    {
        ShopItemContainerUI.onPurchased += ItemPurchasedCallback;
        CurrencyManager.onCurrencyUpdate += CurrencyUpdatedCallback;
    }

    private void OnDestroy() 
    {
        ShopItemContainerUI.onPurchased -= ItemPurchasedCallback;
        CurrencyManager.onCurrencyUpdate -= CurrencyUpdatedCallback;
    }

    public void GameStateChangedCallback(GameState _gameState)
    {
        if(_gameState == GameState.Shop)
        {
            ConfigureShop();
            UpdateRerollVisuals();
        }
    }

    private void ConfigureShop()
    {
        List<GameObject> toDestroy = new List<GameObject>();

        for(int i = 0; i < containersParent.childCount; i++)
        {
            ShopItemContainerUI container = containersParent.GetChild(i).GetComponent<ShopItemContainerUI>();

            if(!container.isLocked)
               toDestroy.Add(container.gameObject);
        }

        while(toDestroy.Count > 0)
        {
            Transform t = toDestroy[0].transform;
            t.SetParent(null);
            Destroy(t.gameObject);
            toDestroy.RemoveAt(0);  
        }


        int containersToAdd = 6 - containersParent.childCount;
        int weaponContainerCount = Random.Range(Mathf.Min(2, containersToAdd), containersToAdd);
        int objectContainerCount = containersToAdd - weaponContainerCount;

        for(int i = 0; i < weaponContainerCount; i++)
        {
            ShopItemContainerUI weaponContainerInstance = Instantiate(shopItemButton, containersParent);
            WeaponDataSO randomWeapon = ResourceManager.GetRandomWeapon();

            weaponContainerInstance.Configure(randomWeapon, Random.Range(0,2));
        }

        for(int i = 0; i < objectContainerCount; i++)
        {
            ShopItemContainerUI objectContainerInstance = Instantiate(shopItemButton, containersParent);
            ObjectDataSO randomObject = ResourceManager.GetRandomObject();

            objectContainerInstance.Configure(randomObject);
        }
    }

    public void Reroll()
    {
        ConfigureShop();
        CurrencyManager.Instance.UseCurrency(rerollPrice);
    }

    private void UpdateRerollVisuals() 
    {
        rerollPriceText.text = rerollPrice.ToString();

        rerollButton.interactable = CurrencyManager.Instance.HasEnoughCurrency(rerollPrice);
    }

    
    private void CurrencyUpdatedCallback()
    {
        UpdateRerollVisuals();
    }

    private void ItemPurchasedCallback(ShopItemContainerUI _shopItemContainerUI, int _weaponLevel)
    {
        if(_shopItemContainerUI.WeaponData != null)
            TryPurchaseWeapon(_shopItemContainerUI, _weaponLevel);
        else 
            PurchaseObject(_shopItemContainerUI);
    }

    private void TryPurchaseWeapon(ShopItemContainerUI _shopItemContainerUI, int _weaponLevel)
    {
        if(characterWeapon.AddWeapon(_shopItemContainerUI.WeaponData, _weaponLevel))
        {
           int price = WeaponStatCalculator.GetPurchasePrice(_shopItemContainerUI.WeaponData, _weaponLevel);
           CurrencyManager.Instance.UseCurrency(price);

           Destroy(_shopItemContainerUI.gameObject);
        }

        onItemPurchased?.Invoke();
    }

    private void PurchaseObject(ShopItemContainerUI _shopItemContainerUI)
    {
        characterObject.AddObject(_shopItemContainerUI.ObjectData);

        CurrencyManager.Instance.UseCurrency(_shopItemContainerUI.ObjectData.Price);

        Destroy(_shopItemContainerUI.gameObject);

        onItemPurchased?.Invoke();
    }
}
 