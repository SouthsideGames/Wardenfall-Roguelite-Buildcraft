using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;
using System;
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

    public GameMode CurrentGameMode { get; private set; }

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
        CurrentGameMode = mode;

        // Notify the WaveManager (if assigned)
        if (waveManager != null)
        {
            waveManager.SetGameMode(mode);
        }

        // Notify all objects implementing IGameModeListener
        NotifyGameModeListeners(mode);
    }

    private void NotifyGameModeListeners(GameMode mode)
    {
        // Find all active objects implementing IGameModeListener
        IEnumerable<IGameModeListener> listeners = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None)
            .OfType<IGameModeListener>();

        // Notify each listener
        foreach (var listener in listeners)
        {
            listener.GameModeChangedCallback(mode);
        }
    }

    
}
