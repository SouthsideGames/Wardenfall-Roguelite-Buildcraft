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
    [Header("ELEMENTS:")]
    [SerializeField] private Transform buttonContainer;
    [SerializeField] private GameModeContainerUI gameModeContainerPrefab;

    [Header("Wave Manager Reference")]
    [SerializeField] private WaveManager waveManager;

    public GameMode CurrentGameMode { get; private set; }

    private GameModeDataSO[] gameModeDatas;

    private void Start()
    {
        buttonContainer.Clear();
        LoadGameModeData();
        GenerateButtons();
    }

    private void LoadGameModeData()
    {
        gameModeDatas = Resources.LoadAll<GameModeDataSO>("Data/Game Mode");
    }

    private void GenerateButtons()
    {
        foreach (var gameModeData in gameModeDatas)
        {
            gameModeData.UpdateUnlockState(); 
            if (gameModeData.IsUnlocked) 
            {
                CreateGameModeButton(gameModeData);
            }
        }
    }

    private void CreateGameModeButton(GameModeDataSO gameModeData)
    {
        GameModeContainerUI containerInstance = Instantiate(gameModeContainerPrefab, buttonContainer);

        containerInstance.Configure(gameModeData);

        containerInstance.GetComponent<Button>().onClick.AddListener(() => SetGameMode(gameModeData));
        containerInstance.GetComponent<Button>().onClick.AddListener(() => GameManager.Instance.StartWeaponSelect());
    }

    private void SetGameMode(GameModeDataSO gameModeData)
    {
        Debug.Log($"Game Mode Set: {gameModeData.Name}");

        if (waveManager != null)
        {
            waveManager.SetGameMode(gameModeData.GameMode);
        }

        NotifyGameModeListeners(gameModeData.GameMode);
    }

    private void NotifyGameModeListeners(GameMode mode)
    {
        IEnumerable<IGameModeListener> listeners = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None)
            .OfType<IGameModeListener>();

        // Notify each listener
        foreach (var listener in listeners)
        {
            listener.GameModeChangedCallback(mode);
        }
    }
    
}
