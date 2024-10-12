using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class WaveTransitionManager : MonoBehaviour, IGameStateListener
{
    [Header("CHARACTER INFO:")]
    [SerializeField] private CharacterStats characterStats;
    [SerializeField] private CharacterObjects characterObjects;

    [Header("ELEMENTS:")]
    [SerializeField] private UpgradeContainerUI[] upgradeContainers;
    [SerializeField] private GameObject upgradeContainersParent;
    
    [Header("CHEST:")]
    [SerializeField] private ChestObjectContainerUI chestObjectContainerUI;
    [SerializeField] private Transform chestContainerParent;

    [Header("SETTINGS:")]
    private int chestsCollected;

    private void Awake()
    {
        Chest.onCollected += ChestCollectedCallback;
    }


    private void OnDestroy() 
    {
        Chest.onCollected -= ChestCollectedCallback;
    }

    public void GameStateChangedCallback(GameState _gameState)
    {
        switch (_gameState)
        {
            case GameState.WaveTransition:
                TryOpenChest();
                break;
        }
    }

    private void TryOpenChest()
    {
        chestContainerParent.Clear();
        
        if(chestsCollected > 0)
            ShowObject();
        else
            ConfigureUpgradeContainers();
    }

    private void ShowObject()
    {
        chestsCollected--;

        upgradeContainersParent.SetActive(false);   

        ObjectDataSO[] objectDatas = ResourceManager.Objects;
        ObjectDataSO randomObjectData = objectDatas[Random.Range(0, objectDatas.Length)];    

        ChestObjectContainerUI containerInstance = Instantiate(chestObjectContainerUI, chestContainerParent);
        containerInstance.Configure(randomObjectData);

        containerInstance.CollectButton.onClick.AddListener(() => CollectButtonCallback(randomObjectData));
    }

    private void CollectButtonCallback(ObjectDataSO _objectToTake)
    {
        characterObjects.AddObject(_objectToTake);

        TryOpenChest();
    }

    [Button]
    private void ConfigureUpgradeContainers()
    {
        upgradeContainersParent.SetActive(true);

        for (int i = 0; i < upgradeContainers.Length; i++)
        {

            int randomStat = Random.Range(0, Enum.GetValues(typeof(Stat)).Length);

            Stat characterStat = (Stat)Enum.GetValues(typeof(Stat)).GetValue(randomStat);

            string randomStatString = Enums.FormatStatName(characterStat);

            string buttonString;
            Action buttonAction = GetActionToPeform(characterStat, out buttonString);

            upgradeContainers[i].Configure(null, randomStatString, buttonString);

            upgradeContainers[i].Button.onClick.RemoveAllListeners();
            upgradeContainers[i].Button.onClick.AddListener(() => buttonAction?.Invoke()); 
            upgradeContainers[i].Button.onClick.AddListener(() => BonusSelectedCallback());  
        }
    }

    private void BonusSelectedCallback()
    {
        GameManager.Instance.WaveCompletedCallback();
    }

    private Action GetActionToPeform(Stat _characterStat, out string _buttonString)    
    {
        _buttonString = "";
        float value;

        value = Random.Range(1, 10);

        switch (_characterStat)
        {
            case Stat.Attack:
                value = Random.Range(1, 10);
                _buttonString = "+" + value.ToString() + "%";
                break;

            case Stat.AttackSpeed:
                value = Random.Range(1, 10);
                _buttonString = "+" + value.ToString() + "%";
                break;

            case Stat.CriticalChance:
                value = Random.Range(1, 10);
                _buttonString = "+" + value.ToString() + "%";
                break;

            case Stat.CriticalPercent:
                value = Random.Range(1f, 2.5f);
                _buttonString = "+" + value.ToString("F2") + "x";
                break;

            case Stat.MoveSpeed:
                value = Random.Range(1, 10);
                _buttonString = "+" + value.ToString() + "%";
                break;

            case Stat.MaxHealth:
                value = Random.Range(1, 5);
                _buttonString = "+" + value;
                break;

            case Stat.Range:
                value = Random.Range(1f, 5f);
                _buttonString = "+" + value.ToString();
                break;

            case Stat.HealthRecoverySpeed:
                value = Random.Range(1, 3);
                _buttonString = "+" + value.ToString() + "%";
                break;

            case Stat.HealthRecoveryValue:
                value = Random.Range(1, 5);
                _buttonString = "+" + value.ToString() + "%";
                break;

            case Stat.Armor:
                value = Random.Range(1, 10);
                _buttonString = "+" + value.ToString() + "%";
                break;

            case Stat.Luck:
                value = Random.Range(1, 10);
                _buttonString = "+" + value.ToString() + "%";
                break;

            case Stat.Dodge:
                value = Random.Range(1, 10);
                _buttonString = "+" + value.ToString() + "%";
                break;

            case Stat.LifeSteal:
                value = Random.Range(1, 10);
                _buttonString = "+" + value.ToString() + "%";
                break;

            case Stat.CriticalResistancePercent:
                value = Random.Range(1, 10);
                _buttonString = "+" + value.ToString("F2") + "x";
                break;

            case Stat.PickupRange:
                value = Random.Range(1, 8);
                _buttonString = "+" + value.ToString() + "%";
                break;

            default:
                return () => Debug.Log("Invalid Stat");
        }

        return () => CharacterStats.Instance.AddStat(_characterStat, value);    
    }

    private void ChestCollectedCallback(Chest chest)
    {
        chestsCollected++;
        Debug.Log("We now have " + chestsCollected + "chest");
    }

}
