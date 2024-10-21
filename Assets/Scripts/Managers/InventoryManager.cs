
using System;
using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour, IGameStateListener
{
    [Header("COMPONENTS:")]
    [SerializeField] private CharacterWeapon characterWeapons;
    [SerializeField] private CharacterObjects characterObjects;

    [Header("ELEMENTS:")]
    [SerializeField] private Transform inventoryItemsParent;
    [SerializeField] private InventoryItemContainerUI inventoryItemContainer;
    [SerializeField] private ShopManagerUI shopManagerUI;
    [SerializeField] private InventoryItemInfoUI inventoryItemInfoUI;

    public void GameStateChangedCallback(GameState _gameState)
    {
        if(_gameState == GameState.Shop)
           Configure();
    }

    private void Configure()
    {
        inventoryItemsParent.Clear();   

        Weapon[] weapons = characterWeapons.GetWeapons();

        for (int i = 0; i < weapons.Length; i++)
        {
            InventoryItemContainerUI container = Instantiate(inventoryItemContainer, inventoryItemsParent);

            container.Configure(weapons[i], () => ShowItemInfo(container));
        }

        ObjectDataSO[] objectDatas = characterObjects.Objects.ToArray();

        for (int i = 0; i < objectDatas.Length; i++)
        {
            InventoryItemContainerUI container = Instantiate(inventoryItemContainer, inventoryItemsParent);

            container.Configure(objectDatas[i], () => ShowItemInfo(container));
        }
    }

    private void ShowItemInfo(InventoryItemContainerUI _container)
    {
        if(_container.Weapon != null)
           ShowWeaponInfo(_container.Weapon);
        else
            ShowObjectInfo(_container.ObjectData);
    }

    private void ShowWeaponInfo(Weapon _weapon)
    {
       inventoryItemInfoUI.Configure(_weapon);
        shopManagerUI.ShowItemInfoPanel();
    }

    private void ShowObjectInfo(ObjectDataSO _object)
    {
        inventoryItemInfoUI.Configure(_object);
        shopManagerUI.ShowItemInfoPanel();
    }

    
}
