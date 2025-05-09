using System.Collections.Generic;
using UnityEngine;

public class BoosterSelectionUI : MonoBehaviour
{
    [SerializeField] private Transform contentParent;
    [SerializeField] private GameObject boosterCardPrefab;

    public void ShowAvailableBoosters(CharacterEquipmentUI equipmentUI)
    {
        foreach (Transform child in contentParent)
            Destroy(child.gameObject);

        foreach (var booster in BoosterRegistry.Instance.allBoosters)
        {
            if (!ProgressionManager.Instance.IsUnlockActive(booster.boosterID))
                continue;

            GameObject go = Instantiate(boosterCardPrefab, contentParent);
            BoosterCardUI cardUI = go.GetComponent<BoosterCardUI>();
            cardUI.Configure(booster, equipmentUI);
        }
    }
}