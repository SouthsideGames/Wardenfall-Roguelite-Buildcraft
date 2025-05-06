using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;
using System;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public static Action OnGamePaused;
    public static Action OnGameResumed; 
    public static Action OnWaveCompleted;
    private Action _postProgressionCallback;

    public GameState gameState { get; private set; }

    private void Awake()
    {
        if(Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }


    void Start()
    {
        Application.targetFrameRate = 60;
        
        if (UserManager.Instance.IsFirstTimePlayer())
        {
            SetGameState(GameState.Username);
            return;
        }
        
        SetGameState(GameState.Menu);
    }

    public void StartGame()
    {
        CurrencyManager.Instance.EarlyInvestorSkillAction();
        SetGameState(GameState.Game);
    }

    public void StartMainMenu() => SetGameState(GameState.Menu);
    public void StartWeaponSelect() => SetGameState(GameState.WeaponSelect);
    public void StartShop() => SetGameState(GameState.Shop);    
    public void StartTraitSelection() => SetGameState(GameState.TraitSelection);
    public void StartCardDraft() => SetGameState(GameState.CardDraft);
    
    public void StartGameOver()
    {
        UIManager.Instance.UpdateGameoverPanel();
        SetGameState(GameState.GameOver); 
    }  
        

    public void SetGameState(GameState _gameState)
    {
        gameState = _gameState;

        IEnumerable<IGameStateListener> gameStateListeners = 
            FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None)
            .OfType<IGameStateListener>();   

        foreach(IGameStateListener gameStateListener in gameStateListeners) 
            gameStateListener.GameStateChangedCallback(_gameState);

    }

    public void WaveCompletedCallback()
    {
        OnWaveCompleted?.Invoke();

        StatisticsManager.Instance.StopTimer();
        StatisticsManager.Instance.EndRun();

        // Always award XP
        int difficultyMultiplier = 1;
        int traitCount = TraitManager.Instance.GetActiveTraitCount();
        int waveNumber = WaveManager.Instance.currentWaveIndex;
        int metaXP = ProgressionXPGranter.CalculateMetaXP(waveNumber, difficultyMultiplier, traitCount);
        ProgressionManager.Instance.AddMetaXP(metaXP);

        // Determine what to do after progression screen
        bool hasLevelUp = CharacterManager.Instance.HasLeveledUp();
        bool hasChest = WaveTransitionManager.Instance.HasCollectedChest();
        bool isBossWave = WaveManager.Instance.IsCurrentWaveBoss();

        _postProgressionCallback = () =>
        {
            if (isBossWave)
                StartTraitSelection();
            else if (hasLevelUp || hasChest)
                SetGameState(GameState.WaveTransition);
            else
                SetGameState(GameState.Shop);
        };

        UIManager.Instance.ShowCharacterProgressionPanel();

    }

    public void RunPostProgressionCallback()
    {
        _postProgressionCallback?.Invoke();
        _postProgressionCallback = null;
    }

    public void ManageGameOver() => SceneManager.LoadScene(0);

    public void PauseButtonCallback()
    {
        Time.timeScale = 0;
        OnGamePaused?.Invoke();
    }

    public void ResumeButtonCallback()
    {
        Time.timeScale = 1;
        OnGameResumed?.Invoke();    
    }

    public void Restart()
    {
        Time.timeScale = 1;
        ManageGameOver();
    }

    private void OnDestroy()
    {
        OnGamePaused = null;
        OnGameResumed = null;
        OnWaveCompleted = null;
    }


    public bool InGameState() => gameState == GameState.Game;

    
 
}


