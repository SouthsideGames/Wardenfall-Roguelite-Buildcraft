using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[CreateAssetMenu(fileName = "Trait Data", menuName = "Scriptable Objects/New Trait Data", order = 7)]
public class TraitDataSO : ScriptableObject
{
    [field: SerializeField] public string TraitID;
    [field: SerializeField] public string DisplayName;
    [field: SerializeField, TextArea] public string Description;
    [field: SerializeField] public Sprite Icon;
    [field: SerializeField] public List<TraitTier> Tiers;


    public TraitTier GetTier(int stackCount) {
        int index = Mathf.Clamp(stackCount - 1, 0, Tiers.Count - 1);
        return Tiers[index];
    }
}
