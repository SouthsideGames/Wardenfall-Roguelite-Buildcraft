using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoosterSelectionUI : MonoBehaviour
{
    [SerializeField] private Transform contentParent;
    [SerializeField] private GameObject boosterCardPrefab;
    [SerializeField] private Button EquipButton;

    public void ShowAvailableBoosters(EquipmentUI equipmentUI)
    {
        foreach (Transform child in contentParent)
            Destroy(child.gameObject);

        foreach (var booster in ProgressionBoosterRegistry.Instance.allBoosters)
        {
            if (!ProgressionManager.Instance.IsUnlockActive(booster.boosterID))
                continue;

            GameObject go = Instantiate(boosterCardPrefab, contentParent);
            BoosterUI cardUI = go.GetComponent<BoosterUI>();
            cardUI.Configure(booster, equipmentUI);

        }
    }
}