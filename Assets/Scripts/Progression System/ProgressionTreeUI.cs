using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ProgressionTreeUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Transform buttonParent;
    [SerializeField] private GameObject unlockButtonPrefab;
    [SerializeField] private GameObject detailPanel;
    [SerializeField] private TextMeshProUGUI detailName;
    [SerializeField] private TextMeshProUGUI detailDescription;
    [SerializeField] private TextMeshProUGUI detailCost;
    [SerializeField] private Button unlockButton;

    private ProgressionUnlockDataSO currentSelection;

    private void Start()
    {
        GenerateUnlockTree();
        CloseDetailPanel();
    }

    public void GenerateUnlockTree()
    {
        foreach (Transform child in buttonParent)
            Destroy(child.gameObject);

        foreach (var unlock in ProgressionManager.Instance.progressionUnlockDatabase.allUnlocks)
        {
            GameObject go = Instantiate(unlockButtonPrefab, buttonParent);
            ProgressionUnlockButtonUI buttonUI = go.GetComponent<ProgressionUnlockButtonUI>();
            buttonUI.Configure(unlock, this);
            
        }
    }

    public void ShowDetail(ProgressionUnlockDataSO unlock)
    {
        currentSelection = unlock;
        detailPanel.SetActive(true);
        detailName.text = unlock.displayName;
        detailDescription.text = unlock.description;
        detailCost.text = $"Cost: {unlock.cost}";

        bool isUnlocked = ProgressionManager.Instance.IsUnlockActive(unlock.unlockID);
        unlockButton.gameObject.SetActive(!isUnlocked);

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