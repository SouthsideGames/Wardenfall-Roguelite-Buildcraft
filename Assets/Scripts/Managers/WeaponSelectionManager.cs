using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;

public class WeaponSelectionManager : MonoBehaviour, IGameStateListener
{
    [Header("ELEMENTS:")]
    [SerializeField] private Transform containersParent;
    [SerializeField] private WeaponSelectionContainerUI weaponContainerPrefab;
    [SerializeField] private Button startButton;
    private CharacterWeapon characterWeapon;

    [Header("DATA:")]
    [SerializeField] private WeaponDataSO[] starterWeapons;
    private WeaponDataSO selectedWeapon;
    private int initialWeaponLevel;

    private void Start()
    {
        characterWeapon = CharacterManager.Instance.weapon;
        startButton.interactable = false;
    }

    private void Update() 
    {
        if(selectedWeapon != null)
          startButton.interactable = true;
    }

    public void GameStateChangedCallback(GameState _gameState)
    {
        switch (_gameState)
        {
            case GameState.Game:
                if (selectedWeapon == null)
                    return;
                characterWeapon.AddWeapon(selectedWeapon, initialWeaponLevel);
                selectedWeapon = null;
                initialWeaponLevel = 0;
                break;
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

        int level = UnityEngine.Random.Range(0,4);

        containerInstance.Configure(weaponData, level);  

        containerInstance.Button.onClick.RemoveAllListeners();
        containerInstance.Button.onClick.AddListener(() => WeaponSelectedCallback(containerInstance, weaponData, level));
    }

    private void WeaponSelectedCallback(WeaponSelectionContainerUI _container, WeaponDataSO _weaponData, int _level)
    {
        selectedWeapon = _weaponData;
        initialWeaponLevel = _level;
        StatisticsManager.Instance.RecordWeaponUsage(_weaponData.ID);

        foreach (WeaponSelectionContainerUI container in containersParent.GetComponentsInChildren<WeaponSelectionContainerUI>())
        {
            if(container == _container)
                container.Select();
            else
                container.Deselect();
        }
    }
    
}
