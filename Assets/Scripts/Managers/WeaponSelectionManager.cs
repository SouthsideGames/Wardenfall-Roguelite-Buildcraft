using System.Collections;
using System.Collections.Generic;
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

    private void Configure()
    {
        containersParent.Clear();

        for (int i = 0; i < 3; i++)
            GenerateWeaponsContainer();
    }

    private void GenerateWeaponsContainer()
    {
        WeaponSelectionContainerUI containerInstance = Instantiate(weaponContainerPrefab, containersParent);

        WeaponDataSO weaponData = starterWeapons[Random.Range(0, starterWeapons.Length)];   

        containerInstance.Configure(weaponData.Icon, weaponData.Name);  
    }
    
}
