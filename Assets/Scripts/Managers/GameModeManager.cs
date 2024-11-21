using UnityEngine;
using UnityEngine.UI;

public class GameModeManager : MonoBehaviour
{
    [Header("Game Mode Buttons")]
    [SerializeField] private Button waveBasedButton;
    [SerializeField] private Button survivalButton;
    [SerializeField] private Button endlessButton;
    [SerializeField] private Button bossRushButton;
    [SerializeField] private Button objectiveBasedButton;

    [Header("Wave Manager Reference")]
    [SerializeField] private WaveManager waveManager;

    private void Awake()
    {
        waveBasedButton.onClick.AddListener(() => SetGameMode(GameMode.WaveBased));
        survivalButton.onClick.AddListener(() => SetGameMode(GameMode.Survival));
        endlessButton.onClick.AddListener(() => SetGameMode(GameMode.Endless));
        bossRushButton.onClick.AddListener(() => SetGameMode(GameMode.BossRush));
        objectiveBasedButton.onClick.AddListener(() => SetGameMode(GameMode.ObjectiveBased));
    }

    private void SetGameMode(GameMode mode)
    {
        if (waveManager == null)
            return;

        waveManager.SetGameMode(mode);
        Debug.Log($"Game mode set to: {mode}");
    }
}
