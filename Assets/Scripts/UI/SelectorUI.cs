using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SelectorUI : MonoBehaviour
{
    [Header("ELEMENTS:")]
    [SerializeField] private EventSystem eventSystem;
    [SerializeField] private Panel shopPanel;

    private void Awake()
    {
        UIManager.OnPanelShown                  += PanelSelectCallback;

        InputManager.OnLock                     += LockCallback;

        ShopManager.OnItemPurchased             += ItemPurchasedCallback;
        ShopManager.OnRerollDisabled            += RerollDisabledCallback;

        ShopManagerUI.OnInventoryOpened         += InventoryOpenedCallback; 
        ShopManagerUI.OnInventoryClosed         += InventoryClosedCallback;
        ShopManagerUI.OnCharacterStatsOpened    += CharacterStatsOpenedCallback; 
        ShopManagerUI.OnCharacterStatsClosed    += CharacterStatsClosedCallback;
        ShopManagerUI.OnItemInfoClosed          += InventoryItemInfoClosedCallback;

        InventoryManager.OnItemInfoOpened       += InventoryItemInfoOpenedCallback;
        InventoryManager.OnItemRecycled         += InventoryItemRecycledCallback;
        InventoryManager.OnWeaponFused          += WeaponFusedCallback;  

        ChestObjectContainerUI.OnSpawned        += ChestSpawnedCallback;
        
        WaveTransitionManager.OnConfigured      += ConfiguredCallback;

        CodexManager.OnDetailsOpen              += DetailOpenCallback;
        CodexManager.OnBigDetailsOpen           += DetailBigDetailsOpenCallback;
        CodexManager.OnBigDetailsClosed         += DetailBigDetailsClosedCallback;

    }

    private void OnDestroy()
    {
        UIManager.OnPanelShown                  -= PanelSelectCallback;

        InputManager.OnLock                     -= LockCallback;
        
        ShopManager.OnItemPurchased             -= ItemPurchasedCallback;
        ShopManager.OnRerollDisabled            -= RerollDisabledCallback; 

        ShopManagerUI.OnInventoryOpened         -= InventoryOpenedCallback; 
        ShopManagerUI.OnInventoryClosed         -= InventoryClosedCallback;
        ShopManagerUI.OnCharacterStatsOpened    -= CharacterStatsOpenedCallback; 
        ShopManagerUI.OnCharacterStatsClosed    -= CharacterStatsClosedCallback;
        ShopManagerUI.OnItemInfoClosed          -= InventoryItemInfoClosedCallback;

        InventoryManager.OnItemInfoOpened       -= InventoryItemInfoOpenedCallback;
        InventoryManager.OnItemRecycled         -= InventoryItemRecycledCallback;
        InventoryManager.OnWeaponFused          -= WeaponFusedCallback;  

        ChestObjectContainerUI.OnSpawned        -= ChestSpawnedCallback;

        WaveTransitionManager.OnConfigured      -= ConfiguredCallback;

        CodexManager.OnDetailsOpen              -= DetailOpenCallback;
        CodexManager.OnBigDetailsOpen           -= DetailBigDetailsOpenCallback;
        CodexManager.OnBigDetailsClosed         -= DetailBigDetailsClosedCallback;
    }


    #region CALLBACKS

    private void LockCallback()
    {
        if(eventSystem.currentSelectedGameObject == null)
           return;
        
        GameObject go = eventSystem.currentSelectedGameObject;

        if(go.TryGetComponent(out ShopItemContainerUI shopItemContainer))
           shopItemContainer.LockButtonCallback();
    }

    private void PanelSelectCallback(Panel _panel)
    {
        if (_panel.FirstSelectedObject != null)
            SetSelectedGameObject(_panel.FirstSelectedObject);
    }

    private void ItemPurchasedCallback() => SelectShopPanelFirstObject();
    private void RerollDisabledCallback() => SelectShopPanelFirstObject(); 

    private void InventoryOpenedCallback()
    {
        UIManager.ShowPanelInteractability(shopPanel.gameObject, false); 

        GameObject selected = InventoryManager.Instance.GetFirstItem();

        if(selected != null)
          SetSelectedGameObject(selected);  
    }

    private void InventoryClosedCallback()
    {
        UIManager.ShowPanelInteractability(shopPanel.gameObject, true);   

        SelectShopPanelFirstObject();
    }

    private void CharacterStatsOpenedCallback() => UIManager.ShowPanelInteractability(shopPanel.gameObject, false); 

    private void CharacterStatsClosedCallback()
    {
        UIManager.ShowPanelInteractability(shopPanel.gameObject, true);   

        SelectShopPanelFirstObject();
    }

    private void InventoryItemRecycledCallback(GameObject _inventoryFirstItem)
    {
        if(_inventoryFirstItem!= null)
          SetSelectedGameObject(_inventoryFirstItem);  

    }

    private void InventoryItemInfoOpenedCallback(Button _recycleButton) => SetSelectedGameObject(_recycleButton.gameObject);

    private void InventoryItemInfoClosedCallback()
    {
        GameObject selected = InventoryManager.Instance.GetFirstItem();

        if(selected != null)
          SetSelectedGameObject(selected);  
    }

    private void WeaponFusedCallback(GameObject _recycleButton) => SetSelectedGameObject(_recycleButton);

    private void ConfiguredCallback(GameObject _upgradeContainer) => SetSelectedGameObject(_upgradeContainer);
    private void ChestSpawnedCallback(GameObject _takeButton) => SetSelectedGameObject(_takeButton);

    private void DetailOpenCallback(GameObject _backButton) => SetSelectedGameObject(_backButton);   

    private void DetailBigDetailsOpenCallback(GameObject _closeButton) => SetSelectedGameObject(_closeButton);   

    private void DetailBigDetailsClosedCallback(GameObject _detailBackButton) => SetSelectedGameObject(_detailBackButton);   

#endregion
 
    private void SetSelectedGameObject(GameObject _go) => eventSystem.SetSelectedGameObject(_go);
    private void SelectShopPanelFirstObject()
    {
        if(shopPanel.FirstSelectedObject != null)
           SetSelectedGameObject(shopPanel.FirstSelectedObject);
    }


}
