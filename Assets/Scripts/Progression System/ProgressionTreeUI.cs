using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ProgressionTreeUI : MonoBehaviour
{
    [Header("UI References")]
    public Transform buttonParent;
    public GameObject unlockButtonPrefab;
    public GameObject detailPanel;
    public TextMeshProUGUI detailName;
    public TextMeshProUGUI detailDescription;
    public TextMeshProUGUI detailCost;
    public Button unlockButton;

    private UnlockDataSO currentSelection;

    private void Start()
    {
        GenerateUnlockTree();
        CloseDetailPanel();
    }

    public void GenerateUnlockTree()
    {
        foreach (Transform child in buttonParent)
            Destroy(child.gameObject);

        foreach (var unlock in UnlockDatabase.Instance.allUnlocks)
        {
            GameObject go = Instantiate(unlockButtonPrefab, buttonParent);
            UnlockButtonUI buttonUI = go.GetComponent<UnlockButtonUI>();
            buttonUI.Configure(unlock, this);
        }
    }

    public void ShowDetail(UnlockDataSO unlock)
    {
        currentSelection = unlock;
        detailPanel.SetActive(true);
        detailName.text = unlock.displayName;
        detailDescription.text = unlock.description;
        detailCost.text = $"Cost: {unlock.cost}";

        unlockButton.interactable = !ProgressionManager.Instance.IsUnlockActive(unlock.unlockID);
        unlockButton.onClick.RemoveAllListeners();
        unlockButton.onClick.AddListener(() => TryUnlockCurrent());
    }

    private void TryUnlockCurrent()
    {
        if (currentSelection == null) return;

        if (ProgressionManager.Instance.TryUnlock(currentSelection.unlockID))
        {
            GenerateUnlockTree();
            ShowDetail(currentSelection);
        }
    }

    public void CloseDetailPanel()
    {
        detailPanel.SetActive(false);
        currentSelection = null;
    }
}