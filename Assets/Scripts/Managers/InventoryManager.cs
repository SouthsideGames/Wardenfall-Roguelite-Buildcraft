
using System;
using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour, IGameStateListener
{
    public static InventoryManager Instance;

    [Header("ACTIONS:")]
    public static Action<Button> OnItemInfoOpened;
    public static Action<GameObject> OnItemRecycled;
    public static Action<GameObject> OnWeaponFused;

    [Header("COMPONENTS:")]
    [SerializeField] private CharacterWeapon characterWeapons;
    [SerializeField] private CharacterObjects characterObjects;

    [Header("ELEMENTS:")]
    [SerializeField] private Transform inventoryItemsParent;
    [SerializeField] private Transform pauseInventoryItemsParent;
    [SerializeField] private InventoryItemContainerUI inventoryItemContainer;
    [SerializeField] private ShopManagerUI shopManagerUI;
    [SerializeField] private InventoryItemInfoUI inventoryItemInfoUI;

    private void Awake() 
    {
        if(Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        ShopManager.OnItemPurchased += ItemPurchasedCallback;
        WeaponFuserManager.onFuse += WeaponFusedCallback;
        GameManager.OnGamePaused += ConfigureInventory;
    }

    private void OnDestroy() 
    {
        ShopManager.OnItemPurchased -= ItemPurchasedCallback;
        WeaponFuserManager.onFuse -= WeaponFusedCallback;
        GameManager.OnGamePaused -= ConfigureInventory;
    }

    public void GameStateChangedCallback(GameState _gameState)
    {
        if(_gameState == GameState.Shop)
           ConfigureInventory();
    }

    private void ConfigureInventory()
    {
        inventoryItemsParent.Clear();   
        pauseInventoryItemsParent.Clear();

        Weapon[] weapons = characterWeapons.GetWeapons();

        for (int i = 0; i < weapons.Length; i++)
        {
            if(weapons[i] == null)
                continue;

            InventoryItemContainerUI container = Instantiate(inventoryItemContainer, inventoryItemsParent);
            container.Configure(weapons[i], i, () => ShowItemInfo(container));

            
            InventoryItemContainerUI pauseContainer = Instantiate(inventoryItemContainer, pauseInventoryItemsParent);
            pauseContainer.Configure(weapons[i], i, () => ShowItemInfo(pauseContainer));
        }

        ObjectDataSO[] objectDatas = characterObjects.Objects.ToArray();

        for (int i = 0; i < objectDatas.Length; i++)
        {
            InventoryItemContainerUI container = Instantiate(inventoryItemContainer, inventoryItemsParent);
            container.Configure(objectDatas[i], () => ShowItemInfo(container));

            InventoryItemContainerUI pauseContainer = Instantiate(inventoryItemContainer, pauseInventoryItemsParent);
            pauseContainer.Configure(objectDatas[i], () => ShowItemInfo(pauseContainer));
        }
    }

    private void ShowItemInfo(InventoryItemContainerUI _container)
    {
        if(_container.Weapon != null)
           ShowWeaponInfo(_container.Weapon, _container.Index);
        else
            ShowObjectInfo(_container.ObjectData);
    }

    private void ShowWeaponInfo(Weapon _weapon, int _index)
    {
       inventoryItemInfoUI.ConfigureInventoryInfo(_weapon);

        inventoryItemInfoUI.RecycleButton.onClick.RemoveAllListeners();
        inventoryItemInfoUI.RecycleButton.onClick.AddListener(() => RecycleWeapon(_index));

        shopManagerUI.ShowItemInfoPanel();
    }

    private void ShowObjectInfo(ObjectDataSO _object)
    {
        inventoryItemInfoUI.Configure(_object);

        inventoryItemInfoUI.RecycleButton.onClick.RemoveAllListeners();
        inventoryItemInfoUI.RecycleButton.onClick.AddListener(() => RecycleObject(_object));

        shopManagerUI.ShowItemInfoPanel();

        OnItemInfoOpened?.Invoke(inventoryItemInfoUI.RecycleButton);
    }

    private void RecycleObject(ObjectDataSO _objectToRecycle)
    {
        characterObjects.RemoveObject(_objectToRecycle);
        ConfigureInventory();
        shopManagerUI.HideItemInfoPanel();  

        OnItemRecycled?.Invoke(GetFirstItem());
    }

    private void RecycleWeapon(int _index)
    {
        characterWeapons.RecycleWeapon(_index);

        ConfigureInventory();

        shopManagerUI.HideItemInfoPanel();
    }
 
    private void ItemPurchasedCallback() => ConfigureInventory();

    private void WeaponFusedCallback(Weapon _fusedWeapon)
    {
        ConfigureInventory(); 
        inventoryItemInfoUI.ConfigureInventoryInfo(_fusedWeapon);
        OnWeaponFused?.Invoke(inventoryItemInfoUI.RecycleButton.gameObject);  
    }

    public GameObject GetFirstItem()
    {
        if(inventoryItemsParent.childCount > 0)
           return inventoryItemsParent.GetChild(0).gameObject;
        
        return null;    
    }


}
