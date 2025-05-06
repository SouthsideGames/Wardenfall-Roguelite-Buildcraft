using System.Collections.Generic;
using UnityEngine;

public class CharacterEquipment : MonoBehaviour
{
    public List<StatBoosterSO> equippedBoosters = new List<StatBoosterSO>();

    private void Awake()
    {
        LoadEquippedBoosters();
    }

    private void LoadEquippedBoosters()
    {
        
        foreach (var booster in BoosterRegistry.Instance.allBoosters)
        {
            if (ProgressionManager.Instance.IsUnlockActive(booster.boosterID))
            {
                equippedBoosters.Add(booster);
            }
        }
    }
}