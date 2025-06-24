using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using SouthsideGames.SaveManager;

public class ChallengeManager : MonoBehaviour, IWantToBeSaved
{
    public static ChallengeManager Instance;

    public static Action<ChallengeMode> OnChallengeChanged;

    [Header("UI Elements")]
    [SerializeField] private Sprite offImage;
    [SerializeField] private Sprite onImage;
    [SerializeField] private Button hardcoreButton;
    [SerializeField] private Button traitChaosButton;
    [SerializeField] private Button rogueRouletteButton;
    [SerializeField] private Button singleSlotButton;

    private ChallengeMode currentMode = ChallengeMode.None;
    private const string challengeKey = "SelectedChallenge";

    private CharacterStats playerStats;
    private Stat lastRouletteStat;
    private float lastRouletteMultiplier;


    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        hardcoreButton.onClick.AddListener(() => SelectChallenge(ChallengeMode.Hardcore));
        traitChaosButton.onClick.AddListener(() => SelectChallenge(ChallengeMode.TraitChaos));
        rogueRouletteButton.onClick.AddListener(() => SelectChallenge(ChallengeMode.RogueRoulette));
        singleSlotButton.onClick.AddListener(() => SelectChallenge(ChallengeMode.SingleSlot));
    }

    private void Start()
    {
        UpdateButtonVisuals();
        OnChallengeChanged?.Invoke(currentMode);
    }

    public ChallengeMode GetCurrentChallenge() => currentMode;

    public static bool IsActive(ChallengeMode mode) => Instance != null && Instance.currentMode == mode;

    public void SelectChallenge(ChallengeMode mode)
    {
        currentMode = (currentMode == mode) ? ChallengeMode.None : mode;

        UpdateButtonVisuals();
        Save();
        OnChallengeChanged?.Invoke(currentMode);
    }

    private void UpdateButtonVisuals()
    {
        SetButtonVisual(hardcoreButton, currentMode == ChallengeMode.Hardcore);
        SetButtonVisual(traitChaosButton, currentMode == ChallengeMode.TraitChaos);
        SetButtonVisual(rogueRouletteButton, currentMode == ChallengeMode.RogueRoulette);
        SetButtonVisual(singleSlotButton, currentMode == ChallengeMode.SingleSlot);
    }

    private void SetButtonVisual(Button button, bool isActive)
    {
        button.image.sprite = isActive ? onImage : offImage;
        var label = button.GetComponentInChildren<TextMeshProUGUI>();
        if (label != null)
            label.text = isActive ? "ON" : "OFF";
    }

    public void Save() => SaveManager.Save(this, challengeKey, (int)currentMode);

    public void Load()
    {
        if (SaveManager.TryLoad(this, challengeKey, out object modeObject))
        {
            currentMode = (ChallengeMode)(int)modeObject;
            UpdateButtonVisuals();
        }
    }

    public void ApplyWaveRouletteEffect(CharacterStats stats)
    {
        playerStats = stats;

        if (currentMode != ChallengeMode.RogueRoulette || playerStats == null)
            return;

        var statValues = Enum.GetValues(typeof(Stat));
        Stat selectedStat = (Stat)statValues.GetValue(UnityEngine.Random.Range(0, statValues.Length));
        float multiplier = UnityEngine.Random.value > 0.5f ? 1.5f : 0.5f;

        playerStats.ApplyTemporaryModifier(selectedStat, multiplier, 30f);

        Debug.Log($"[RogueRoulette] Applied {selectedStat} x{multiplier} for 30s");
    }

    public void SetLastRouletteEffect(Stat stat, float multiplier)
    {
        lastRouletteStat = stat;
        lastRouletteMultiplier = multiplier;
    }

    public bool TryGetLastRouletteEffect(out Stat stat, out float multiplier)
    {
        stat = lastRouletteStat;
        multiplier = lastRouletteMultiplier;
        return true;
    }
}
