using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Stat Icons", menuName = "Scriptable Objects/New Stat Icons", order = 5)]
public class StatIconDataSO : ScriptableObject
{
    [field: SerializeField] public StatIcon[] StatIcons { get; private set; }
}

[System.Serializable]
public struct StatIcon
{
    public Stat stat;
    public Sprite icon;
}
