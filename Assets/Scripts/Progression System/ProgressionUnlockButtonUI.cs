using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ProgressionUnlockButtonUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI label;
    [SerializeField] private TextMeshProUGUI cost;
    [SerializeField] private Image icon;
    [SerializeField] private Image lockIcon;
    [SerializeField] private Button button;

    private ProgressionUnlockDataSO data;
    private ProgressionTreeUI treeUI;

    public void Configure(ProgressionUnlockDataSO unlockData, ProgressionTreeUI manager)
    {
        data = unlockData;
        treeUI = manager;

        label.text = data.displayName;
        cost.text = data.cost.ToString();
        icon.sprite = data.icon;

        ProgressionManager.OnUnlockPointsChanged += UpdateVisualState;
        UpdateVisualState();

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => treeUI.ShowDetail(data));
    }

    private void UpdateVisualState()
    {
        bool unlocked = ProgressionManager.Instance.IsUnlockActive(data.unlockID);
        
        // Show/hide the lock icon based on unlock status
        if (lockIcon != null)
            lockIcon.gameObject.SetActive(!unlocked);
    }

    private void OnDestroy()
    {
        ProgressionManager.OnUnlockPointsChanged -= UpdateVisualState;
    }
}