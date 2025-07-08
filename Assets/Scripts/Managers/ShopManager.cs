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
    public static Action OnItemPurchased;
    public static Action OnRerollDisabled;

    [Header("ELEMENTS:")]
    [SerializeField] private Transform containersParent;
    [SerializeField] private ShopItemContainerUI shopItemButton;
    [SerializeField] private Button rerollButton;
    [SerializeField] private int rerollPrice;
    [SerializeField] private TextMeshProUGUI rerollPriceText;

    [Header("SETTINGS")]
    [SerializeField] private float scrollSpeed;

    [Header("PLAYER COMPONENTS:")]
    [SerializeField] private CharacterWeapon characterWeapon;
    [SerializeField] private CharacterObjects characterObject;

    private void Start()
    {
        ShopItemContainerUI.onPurchased += ItemPurchasedCallback;
        CurrencyManager.onCurrencyUpdate += CurrencyUpdatedCallback;
    }

    private void OnEnable()
    {
        // Ensure we don't double-subscribe
        CurrencyManager.onCurrencyUpdate -= CurrencyUpdatedCallback;
        CurrencyManager.onCurrencyUpdate += CurrencyUpdatedCallback;
    }

    private void OnDestroy()
    {
        ShopItemContainerUI.onPurchased -= ItemPurchasedCallback;
        CurrencyManager.onCurrencyUpdate -= CurrencyUpdatedCallback;
 
    }

    public void GameStateChangedCallback(GameState _gameState)
    {
        if (_gameState == GameState.Shop)
        {
            ConfigureShop();
            UpdateRerollVisuals();
        }
    }

    private void ConfigureShop()
    {
        List<GameObject> toDestroy = new List<GameObject>();

        for (int i = 0; i < containersParent.childCount; i++)
        {
            ShopItemContainerUI container = containersParent.GetChild(i).GetComponent<ShopItemContainerUI>();
            if (!container.isLocked)
                toDestroy.Add(container.gameObject);
        }

        while (toDestroy.Count > 0)
        {
            Transform t = toDestroy[0].transform;
            t.SetParent(null);
            Destroy(t.gameObject);
            toDestroy.RemoveAt(0);
        }

        int containersToAdd = (ProgressionManager.Instance.progressionEffectManager.HasExtraShelf ? 7 : 6) - containersParent.childCount;
        int weaponContainerCount = Random.Range(Mathf.Min(2, containersToAdd), containersToAdd);
        int objectContainerCount = containersToAdd - weaponContainerCount;

        for (int i = 0; i < weaponContainerCount; i++)
        {
            ShopItemContainerUI weaponContainerInstance = Instantiate(shopItemButton, containersParent);
            WeaponDataSO randomWeapon = ResourceManager.GetRandomWeapon();
            weaponContainerInstance.Configure(randomWeapon, Random.Range(0, 2));
        }

        for (int i = 0; i < objectContainerCount; i++)
        {
            ShopItemContainerUI objectContainerInstance = Instantiate(shopItemButton, containersParent);
            ObjectDataSO randomObject = ResourceManager.GetRandomObject();
            objectContainerInstance.Configure(randomObject);
        }
    }

    public void Reroll()
    {
        int effectiveRerollCost = ProgressionManager.Instance.progressionEffectManager.HasFreeOrDiscountReroll ? Mathf.Max(0, rerollPrice - 1) : rerollPrice;
        CurrencyManager.Instance.UseCurrency(effectiveRerollCost);
        ConfigureShop();
    }

    private void UpdateRerollVisuals()
    {
        if (rerollPriceText == null || rerollButton == null)
        {
            Debug.LogWarning("ShopManager: UI references missing during UpdateRerollVisuals.");
            return;
        }

        int effectiveRerollCost = ProgressionManager.Instance.progressionEffectManager.HasFreeOrDiscountReroll 
            ? Mathf.Max(0, rerollPrice - 1) 
            : rerollPrice;

        rerollPriceText.text = effectiveRerollCost.ToString();
        rerollButton.interactable = CurrencyManager.Instance.HasEnoughCurrency(effectiveRerollCost);

        if (!rerollButton.interactable)
            OnRerollDisabled?.Invoke();
    }

    private void CurrencyUpdatedCallback() => UpdateRerollVisuals();

    private void ItemPurchasedCallback(ShopItemContainerUI _shopItemContainerUI, int _weaponLevel)
    {
        if (_shopItemContainerUI.WeaponData != null)
            TryPurchaseWeapon(_shopItemContainerUI, _weaponLevel);
        else
            PurchaseObject(_shopItemContainerUI);
    }

    private void TryPurchaseWeapon(ShopItemContainerUI _shopItemContainerUI, int _weaponLevel)
    {
        if (characterWeapon.AddWeapon(_shopItemContainerUI.WeaponData, _weaponLevel))
        {
            int basePrice = WeaponStatCalculator.GetPurchasePrice(_shopItemContainerUI.WeaponData, _weaponLevel);
            int finalPrice = Mathf.FloorToInt(basePrice * ProgressionManager.Instance.progressionEffectManager.ShopDiscount);
            CurrencyManager.Instance.UseCurrency(finalPrice);

            Destroy(_shopItemContainerUI.gameObject);
        }

        CrowdReactionType reaction = UnityEngine.Random.value < 0.7f
            ? CrowdReactionType.Whistle
            : CrowdReactionType.Laugh;

        AudioManager.Instance?.PlayCrowdReaction(reaction);
        
        OnItemPurchased?.Invoke();
    }

    private void PurchaseObject(ShopItemContainerUI _shopItemContainerUI)
    {
        characterObject.AddObject(_shopItemContainerUI.ObjectData);

        int basePrice = _shopItemContainerUI.ObjectData.Price;
        int finalPrice = Mathf.FloorToInt(basePrice * ProgressionManager.Instance.progressionEffectManager.ShopDiscount);
        CurrencyManager.Instance.UseCurrency(finalPrice);

        Destroy(_shopItemContainerUI.gameObject);

        OnItemPurchased?.Invoke();
    }

}
 