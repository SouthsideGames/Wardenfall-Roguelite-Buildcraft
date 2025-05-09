using UnityEngine;

[CreateAssetMenu(fileName = "Stat Booster Data", menuName = "Scriptable Objects/New Stat Booster Data", order = 12)]
public class StatBoosterSO : ScriptableObject
{
    public Sprite icon;
    public string boosterID;
    public Stat targetStat; 
    public float bonusValue;       
}
