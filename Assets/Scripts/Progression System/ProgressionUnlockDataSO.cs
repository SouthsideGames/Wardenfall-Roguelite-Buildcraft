using UnityEngine;

[CreateAssetMenu(fileName = "Unlock Data", menuName = "Scriptable Objects/New Unlock Data", order = 11)]
public class ProgressionUnlockDataSO : ScriptableObject
{
    public string unlockID;
    public string displayName;
    public string description;
    public int cost;
    public Sprite icon;
    public ProgressionUnlockCategory category;
    public string[] requiredUnlocks;
    public int requiredLevel;
}