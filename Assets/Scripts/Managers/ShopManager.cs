using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopManager : MonoBehaviour, IGameStateListener
{
    [Header("ELEMENTS:")]
    [SerializeField] private Transform containersParent;
    [SerializeField] private ShopItemContainerUI shopItemButton;

    public void GameStateChangedCallback(GameState _gameState)
    {
        if(_gameState == GameState.Shop)
           Configure();
    }

    private void Configure()
    {
        containersParent.Clear();

        int containersToAdd = 6;
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
}
 