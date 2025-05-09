using UnityEngine;

[CreateAssetMenu(fileName = "Unlock Data", menuName = "Scriptable Objects/New Unlock Data", order = 11)]
public class UnlockDataSO : ScriptableObject
{
    public string unlockID;
    public string displayName;
    public string description;
    public int cost;
    public Sprite icon;
    public UnlockCategory category;
    public string[] requiredUnlocks;
    public int requiredLevel;
}