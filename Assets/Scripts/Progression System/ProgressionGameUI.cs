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
    [SerializeField] private GameObject levelUpArrow;

    private void Start()
    {
        levelUpArrow.SetActive(false);
    }

    private void OnEnable()
    {
        if (ProgressionManager.Instance != null)
            ProgressionManager.Instance.OnLevelUp += HandleLevelUp;
    }

    private void OnDisable()
    {
        if (ProgressionManager.Instance != null)
            ProgressionManager.Instance.OnLevelUp -= HandleLevelUp;
    }

    private void HandleLevelUp(int newLevel)
    {
        levelUpArrow.SetActive(true);
    }

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

    public void OnContinuePressed()
    {

        StatisticsManager.Instance.StopTimer();
        StatisticsManager.Instance.EndRun();

        // Always award XP
        int difficultyMultiplier = 1;
        int traitCount = TraitManager.Instance.GetActiveTraitCount();
        int waveNumber = WaveManager.Instance.currentWaveIndex;
        int metaXP = ProgressionXPGranter.CalculateMetaXP(waveNumber, difficultyMultiplier, traitCount);
        ProgressionManager.Instance.AddXP(metaXP);

        bool isBossWave = WaveManager.Instance.IsCurrentWaveBoss();
        bool hasChest = WaveTransitionManager.Instance.HasCollectedChest();
        bool hasLevelUp = CharacterManager.Instance.HasLeveledUp();

        if (hasChest || hasLevelUp)
        {
            GameManager.Instance.SetGameState(GameState.WaveTransition);
        }
        else if (isBossWave)
        {
            GameManager.Instance.SetGameState(GameState.TraitSelection);
        }
        else
        {
            GameManager.Instance.StartShop();
        }
    }
}