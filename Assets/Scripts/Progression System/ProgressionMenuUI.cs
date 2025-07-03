using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ProgressionMenuUI : MonoBehaviour
{
    [SerializeField] private GameObject unlockTreePanel;
    [SerializeField] private TextMeshProUGUI usernameText;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private Slider xpSlider;
    [SerializeField] private GameObject alertObject;

    void Start() => UpdateInfo();


    public void OnOpenUnlockTree()
    {
        unlockTreePanel.SetActive(true);
        unlockTreePanel.GetComponent<ProgressionUnlockTreeUI>()?.Refresh();
        alertObject.SetActive(false);
    }

    public void OnCloseUnlockTree() => unlockTreePanel.SetActive(false);

    public void UpdateInfo()
    {
        var progression = ProgressionManager.Instance;

        usernameText.text = UserManager.Instance.Username;
        levelText.text = progression.PlayerLevel.ToString();

        float currentXP = progression.ProgressionXP;
        float requiredXP = progression.GetXPForNextLevel();
        xpSlider.value = currentXP / requiredXP;

        CheckForUnlockableNodes();
    }
    
    private void CheckForUnlockableNodes()
    {
        bool canUnlock = ProgressionManager.Instance.HasUnlockableNodes();
        alertObject.SetActive(canUnlock);
    }
}