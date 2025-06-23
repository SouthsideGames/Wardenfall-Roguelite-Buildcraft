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

    private ChallengeMode currentMode = ChallengeMode.None;
    private const string challengeKey = "SelectedChallenge";

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        hardcoreButton.onClick.AddListener(() => SelectChallenge(ChallengeMode.Hardcore));
        traitChaosButton.onClick.AddListener(() => SelectChallenge(ChallengeMode.TraitChaos));
    }

    private void Start()
    {
        UpdateButtonVisuals();
        OnChallengeChanged?.Invoke(currentMode);
    }

    public ChallengeMode GetCurrentChallenge() => currentMode;

    public static bool IsActive(ChallengeMode mode) => Instance.currentMode == mode;

    public void SelectChallenge(ChallengeMode mode)
    {
        if (currentMode == mode)
            currentMode = ChallengeMode.None; // Toggle off if already selected
        else
            currentMode = mode;

        UpdateButtonVisuals();
        Save();
        OnChallengeChanged?.Invoke(currentMode);
    }

    private void UpdateButtonVisuals()
    {
        SetButtonVisual(hardcoreButton, currentMode == ChallengeMode.Hardcore);
        SetButtonVisual(traitChaosButton, currentMode == ChallengeMode.TraitChaos);
    }

    private void SetButtonVisual(Button button, bool isActive)
    {
        button.image.sprite = isActive ? onImage : offImage;
        var label = button.GetComponentInChildren<TextMeshProUGUI>();
        if (label != null)
            label.text = isActive ? "ON" : "OFF";
    }

    public void Save()
    {
        SaveManager.Save(this, challengeKey, (int)currentMode);
    }

    public void Load()
    {
        if (SaveManager.TryLoad(this, challengeKey, out object modeObject))
        {
            currentMode = (ChallengeMode)(int)modeObject;
            UpdateButtonVisuals();
        }
    }
}
