using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CharacterProgressionUI : MonoBehaviour
{
    public GameObject panel;
    public TextMeshProUGUI xpText;
    public TextMeshProUGUI pointsText;
    public TextMeshProUGUI levelText;
    public Button continueButton;

    private void Awake()
    {
        continueButton.onClick.AddListener(OnContinuePressed);
        panel.SetActive(false);
    }

    public void Refresh()
    {
        xpText.text = $"XP Gained: {MetaProgressionManager.Instance.LastGainedXP}";
        pointsText.text = $"Unlock Points: {MetaProgressionManager.Instance.UnlockPoints}";
        levelText.text = $"Player Level: {MetaProgressionManager.Instance.PlayerLevel}";
        panel.SetActive(true);
    }

    private void OnContinuePressed()
    {
        panel.SetActive(false);
        GameManager.Instance.RunPostProgressionCallback();
    }
}
