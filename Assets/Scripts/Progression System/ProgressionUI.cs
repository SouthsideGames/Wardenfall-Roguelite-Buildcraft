using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ProgressionUI : MonoBehaviour
{
    [SerializeField] private GameObject unlockTreePanel;
    [SerializeField] private TextMeshProUGUI usernameText;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private Slider xpSlider;

    void Awake() => UpdateInfo();


    public void OnOpenUnlockTree()
    {
        unlockTreePanel.SetActive(true);
        unlockTreePanel.GetComponent<UnlockTreeUI>()?.Refresh();
    }

    public void OnCloseUnlockTree() => unlockTreePanel.SetActive(false);

    private void UpdateInfo()
    {
        var progression = ProgressionManager.Instance;

        usernameText.text = UserManager.Instance.Username;  
        levelText.text = progression.PlayerLevel.ToString();

        float currentXP = progression.ProgressionXP;
        float requiredXP = progression.GetXPForNextLevel();
        xpSlider.value = currentXP / requiredXP;
    }
}