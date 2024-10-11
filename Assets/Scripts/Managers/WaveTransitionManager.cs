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
    [Header("ELEMENTS:")]
    [SerializeField] private UpgradeContainerUI[] upgradeContainers;

    public void GameStateChangedCallback(GameState _gameState)
    {
        switch (_gameState)
        {
            case GameState.WaveTransition:
                ConfigureUpgradeContainers();
                break;
        }
    }

    [Button]
    private void ConfigureUpgradeContainers()
    {
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

}
