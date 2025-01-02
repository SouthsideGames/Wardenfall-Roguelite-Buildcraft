using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameMode Data", menuName = "Scriptable Objects/New Game Mode Data", order = 9)]
public class GameModeDataSO : ScriptableObject
{
    [field: SerializeField] public Sprite Icon { get; private set; }
    [field: SerializeField] public string Name { get; private set; }
    [field: SerializeField, TextArea] public string Description { get; private set; }
    [field: SerializeField] public int NumberOfWavesToUnlock { get; private set; }   
    [field: SerializeField] public GameMode GameMode { get; private set; }   
    [SerializeField] private bool isUnlocked; 

    public bool IsUnlocked => isUnlocked;

    public void UpdateUnlockState() => isUnlocked = StatisticsManager.Instance.currentStatistics.TotalWavesCompleted >= NumberOfWavesToUnlock;

}
