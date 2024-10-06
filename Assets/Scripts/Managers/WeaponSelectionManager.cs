using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

public class WeaponSelectionManager : MonoBehaviour, IGameStateListener
{
    [Header("ELEMENTS:")]
    [SerializeField] private Transform containersParent;
    [SerializeField] private WeaponSelectionContainerUI weaponContainerPrefab;

    [Header("DATA:")]
    [SerializeField] private WeaponDataSO[] starterWeapons;

    public void GameStateChangedCallback(GameState _gameState)
    {
        switch (_gameState)
        {
            case GameState.WeaponSelect:
                Configure();
                break;
        }
    }

    [Button]
    private void Configure()
    {
        containersParent.Clear();

        for (int i = 0; i < 3; i++)
            GenerateWeaponsContainer();

    }


    private void GenerateWeaponsContainer()
    {
        WeaponSelectionContainerUI containerInstance = Instantiate(weaponContainerPrefab, containersParent);

        WeaponDataSO weaponData = starterWeapons[UnityEngine.Random.Range(0, starterWeapons.Length)];   

        int level = UnityEngine.Random.Range(0,2);

        containerInstance.Configure(weaponData.Icon, weaponData.Name, level);  

        containerInstance.Button.onClick.RemoveAllListeners();
        containerInstance.Button.onClick.AddListener(() => WeaponSelectedCallback(containerInstance, weaponData));

    }

    private void WeaponSelectedCallback(WeaponSelectionContainerUI _container, WeaponDataSO _weaponData)
    {
        foreach (WeaponSelectionContainerUI container in containersParent.GetComponentsInChildren<WeaponSelectionContainerUI>())
        {
            if(container == _container)
                container.Select();
            else
                container.Deselect();
        }
    }
    
}
