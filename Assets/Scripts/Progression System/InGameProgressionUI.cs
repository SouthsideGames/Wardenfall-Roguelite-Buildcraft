using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class InGameProgressionUI : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Image characterPortrait;
    [SerializeField] private Slider xpSlider;
    [SerializeField] private TextMeshProUGUI xpText;
    [SerializeField] private TextMeshProUGUI pointsText;
    [SerializeField] private TextMeshProUGUI levelText;

    public void Refresh()
    {
        ProgressionManager mp = ProgressionManager.Instance;

        characterPortrait.sprite = CharacterManager.Instance.CurrentCharacter.Icon;

        float xpThisLevel = mp.ProgressionXP;
        float xpNeeded = mp.GetXPForNextLevel();
        float percent = xpThisLevel / xpNeeded;
        xpSlider.value = 0f;

        LeanTween.value(xpSlider.gameObject, 0f, percent, 0.6f)
            .setEaseOutExpo()
            .setOnUpdate(val => xpSlider.value = val);

        xpText.text = $"XP: {mp.LastGainedXP} / {mp.GetXPForNextLevel()}";

        levelText.text = $"Level {mp.PlayerLevel}";
        LeanTween.scale(levelText.gameObject, Vector3.one * 1.1f, 0.3f).setEasePunch();

        pointsText.text = $"Unlock Points: {mp.UnlockPoints}";
        LeanTween.scale(pointsText.gameObject, Vector3.one * 1.1f, 0.3f).setEasePunch();
    }

    public void OnContinuePressed() => GameManager.Instance.RunPostProgressionCallback();
}