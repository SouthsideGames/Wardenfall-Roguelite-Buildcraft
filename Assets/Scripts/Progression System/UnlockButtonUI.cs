using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UnlockButtonUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI label;
    [SerializeField] private TextMeshProUGUI cost;
    [SerializeField] private Image background;
    [SerializeField] private Image icon;
    [SerializeField] private Color lockedColor;
    [SerializeField] private Color unlockedColor;

    private UnlockDataSO data;
    private ProgressionTreeUI treeUI;

    public void Configure(UnlockDataSO unlockData, ProgressionTreeUI manager)
    {
        data = unlockData;
        treeUI = manager;

        label.text = data.displayName;
        cost.text = $"Cost: {data.cost}";
        icon.sprite = data.icon;

        bool unlocked = ProgressionManager.Instance.IsUnlockActive(data.unlockID);
        background.color = unlocked ? unlockedColor : lockedColor;

        GetComponent<Button>().onClick.RemoveAllListeners();
        GetComponent<Button>().onClick.AddListener(() => treeUI.ShowDetail(data));
    }
}