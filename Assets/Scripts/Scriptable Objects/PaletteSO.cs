using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Palette", menuName = "Scriptable Objects/New Palette", order = -10)]
public class PaletteSO : ScriptableObject
{
    [field: SerializeField] public Color[] LevelColors { get; private set; }
    [field: SerializeField] public Color[] LevelOutlineColors { get; private set; }
    [field: SerializeField] public Color[] cardDetailColors { get; private set; }
}
