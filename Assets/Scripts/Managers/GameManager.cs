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

    [HideInInspector] public float runStartTime;

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
            UIManager.Instance.ShowFirstTimeUI();
            return;
        }

        SetGameState(GameState.Menu);
    }

    public void StartIntro()
    {
        SetGameState(GameState.Intro);
    }

    public void StartGame()
    {
        runStartTime = Time.time;
        CurrencyManager.Instance.EarlyInvestorSkillAction();
        SetGameState(GameState.Game);
    }

    public void StartMainMenu()
    {
        SetGameState(GameState.Menu);
        UIManager.Instance.ShowNextTutorialStep();
    }
    public void StartWeaponSelect() => SetGameState(GameState.WeaponSelect);
    public void StartShop() => SetGameState(GameState.Shop);    
    public void StartTraitSelection() => SetGameState(GameState.TraitSelection);
    public void StartCardDraft() => SetGameState(GameState.CardDraft);

    public void StartGameOver()
    {
        // Update final statistics
        var stats = StatisticsManager.Instance.currentStatistics;
        stats.CurrentRunDuration = Time.time - runStartTime;
        stats.MostUsedCardInRun = CardDraftManager.Instance.cardEffectManager.GetMostUsedCard();
        stats.MostEffectiveWeaponInRun = CharacterManager.Instance.weapon.GetMostEffectiveWeapon();

        UIManager.Instance.UpdateGameoverPanel();
        SetGameState(GameState.GameOver); 
    }


    public void SetGameState(GameState _gameState)
    {
        gameState = _gameState;

        IEnumerable<IGameStateListener> gameStateListeners =
            FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None)
            .OfType<IGameStateListener>();

        foreach (IGameStateListener gameStateListener in gameStateListeners)
            gameStateListener.GameStateChangedCallback(_gameState);
            
        if (_gameState == GameState.Progression)
        {
            UIManager.Instance.ShowCharacterProgressionPanel();
        }

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