using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

        containerInstance.Button.onClick.RemoveAllListeners();
        containerInstance.Button.onClick.AddListener(() => WeaponSelectedCallback(containerInstance, weaponData));
    }

    private void WeaponSelectedCallback(WeaponSelectionContainerUI _weaponContainer, WeaponDataSO _weaponData)
    {

        foreach (WeaponSelectionContainerUI container in containersParent.GetComponentsInChildren<WeaponSelectionContainerUI>())
        {
            if (container == _weaponContainer)
                container.Select();
            else
                container.Deselect();
        }
    }


    
}
