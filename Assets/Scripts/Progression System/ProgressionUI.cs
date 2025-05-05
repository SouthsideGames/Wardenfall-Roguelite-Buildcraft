using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ProgressionUI : MonoBehaviour
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
    
        float xpThisLevel = mp.MetaXP % 1000;
        float percent = xpThisLevel / 1000f;
        xpSlider.value = 0f;

        LeanTween.value(xpSlider.gameObject, 0f, percent, 0.6f)
            .setEaseOutExpo()
            .setOnUpdate(val => xpSlider.value = val);

        xpText.text = mp.LastGainedXP.ToString();

        levelText.text = mp.PlayerLevel.ToString();
        LeanTween.scale(levelText.gameObject, Vector3.one * 1.1f, 0.3f).setEasePunch();

        pointsText.text = $"Unlock Points: {mp.UnlockPoints}";
        LeanTween.scale(pointsText.gameObject, Vector3.one * 1.1f, 0.3f).setEasePunch();
    }

    public void OnContinuePressed() => GameManager.Instance.RunPostProgressionCallback();
}
