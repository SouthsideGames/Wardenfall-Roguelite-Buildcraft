using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Mission Data", menuName = "Scriptable Objects/New Mission Data", order = 0)]
public class MissionDataSO : ScriptableObject
{
    [SerializeField] private MissionType type;
    public MissionType Type => type;

    [SerializeField] private int target;
    public int Target => target;    

    [SerializeField] private int rewardXp;
    public int RewardXp => rewardXp;

    [SerializeField] private Sprite icon;
    public Sprite Icon => icon; 
}
