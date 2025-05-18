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
    public static WaveTransitionManager Instance;

    public static Action<GameObject> OnConfigured;

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
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        Chest.OnCollected += ChestCollectedCallback;
    }


    private void OnDestroy() => Chest.OnCollected -= ChestCollectedCallback;

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
        containerInstance.RecycleButton.onClick.AddListener(() => RecycleButtonCallback(randomObjectData));


    }

    private void CollectButtonCallback(ObjectDataSO _objectToTake)
    {
        characterObjects.AddObject(_objectToTake);

        TryOpenChest();
    }

    private void RecycleButtonCallback(ObjectDataSO _objectToRecycle)
    {
        CurrencyManager.Instance.AdjustCurrency(_objectToRecycle.RecyclePrice);
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

            Sprite upgradeSprite = ResourceManager.GetStatIcon(characterStat);

            string randomStatString = Enums.FormatStatName(characterStat);

            string buttonString;
            Action buttonAction = GetActionToPeform(characterStat, out buttonString);

            upgradeContainers[i].Configure(upgradeSprite, randomStatString, buttonString);

            upgradeContainers[i].Button.onClick.RemoveAllListeners();
            upgradeContainers[i].Button.onClick.AddListener(() => buttonAction?.Invoke()); 
            upgradeContainers[i].Button.onClick.AddListener(() => BonusSelectedCallback());  
        }

        OnConfigured?.Invoke(upgradeContainers[0].gameObject);
    }

    private IEnumerator WaitAndShowTraitSelection() 
    {
        yield return new WaitForSeconds(0.5f); 
        TraitSelectionManager.Instance.OpenTraitSelection(); 
    }

    private void BonusSelectedCallback() => GameManager.Instance.WaveCompletedCallback();

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

            case Stat.CritChance:
                value = Random.Range(1, 10);
                _buttonString = "+" + value.ToString() + "%";
                break;

            case Stat.CritDamage:
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

            case Stat.RegenSpeed:
                value = Random.Range(1, 3);
                _buttonString = "+" + value.ToString() + "%";
                break;

            case Stat.RegenValue:
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

            case Stat.CritResist:
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

        return () => CharacterManager.Instance.stats.AddStat(_characterStat, value);    
    }

    private void ChestCollectedCallback(Chest chest) => chestsCollected++;

    public bool HasCollectedChest() => chestsCollected > 0;


}
