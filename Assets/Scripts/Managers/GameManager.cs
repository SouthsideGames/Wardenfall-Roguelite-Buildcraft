using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;
using System;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public static Action onGamePaused;
    public static Action onGameResumed; 

    private void Awake()
    {
        if(Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 60;
        SetGameState(GameState.Menu);
    }

    public void StartGame() => SetGameState(GameState.Game);    
    public void StartWeaponSelect() => SetGameState(GameState.WeaponSelect);
    public void StartShop() => SetGameState(GameState.Shop);    
    public void StartGameOver() => SetGameState(GameState.GameOver);  

    public void SetGameState(GameState _gameState)
    {
        IEnumerable<IGameStateListener> gameStateListeners = 
            FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None)
            .OfType<IGameStateListener>();   

        foreach(IGameStateListener gameStateListener in gameStateListeners) 
          gameStateListener.GameStateChangedCallback(_gameState);

    }

    public void WaveCompletedCallback()
    {
        if(CharacterManager.Instance.HasLeveledUp() || WaveTransitionManager.Instance.HasCollectedChest())
        {
            SetGameState(GameState.WaveTransition);
        }
        else
        {
            SetGameState(GameState.Shop);
        }
    }

    public void ManageGameOver()
    {
        SceneManager.LoadScene(0);
    }

    public void PauseButtonCallback()
    {
        Time.timeScale = 0;
        onGamePaused?.Invoke();
    }

    public void ResumeButtonCallback()
    {
        Time.timeScale = 1;
        onGameResumed?.Invoke();    
    }
}


