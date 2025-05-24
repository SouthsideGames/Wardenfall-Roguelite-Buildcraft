using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ProgressionGameUI : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Image characterPortrait;
    [SerializeField] private Slider xpSlider;
    [SerializeField] private TextMeshProUGUI xpText;
    [SerializeField] private TextMeshProUGUI pointsText;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private AudioClip xpGainSound;

    public void Refresh()
    {
        ProgressionManager mp = ProgressionManager.Instance;

        characterPortrait.sprite = CharacterManager.Instance.CurrentCharacter.Icon;

        float xpThisLevel = mp.ProgressionXP;
        float xpNeeded = mp.GetXPForNextLevel();
        float percent = xpThisLevel / xpNeeded;

        xpSlider.value = 0f;
        LeanTween.cancel(xpSlider.gameObject);

        int xpStart = 0;
        int xpEnd = mp.LastGainedXP;
        
        AudioManager.Instance.PlaySFX(xpGainSound);

        LeanTween.delayedCall(0.15f, () =>
        {
            // Animate slider
            LeanTween.value(xpSlider.gameObject, 0f, percent, 0.6f)
                .setEaseOutExpo()
                .setOnUpdate(val => xpSlider.value = val);

            // Animate XP text count-up
            LeanTween.value(xpText.gameObject, xpStart, xpEnd, 0.6f)
                .setEaseOutExpo()
                .setOnUpdate((float val) =>
                {
                    xpText.text = $"XP: {(int)val} / {mp.GetXPForNextLevel()}";
                });
        });

        levelText.text = mp.PlayerLevel.ToString();
        LeanTween.scale(levelText.gameObject, Vector3.one * 1.1f, 0.3f).setEasePunch();

        pointsText.text = $"SIGILS: {mp.UnlockPoints}";
        LeanTween.scale(pointsText.gameObject, Vector3.one * 1.1f, 0.3f).setEasePunch();
    }

    public void OnContinuePressed() => GameManager.Instance.RunPostProgressionCallback();
}