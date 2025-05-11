using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System;
using SouthsideGames.SaveManager;

public class UIManager : MonoBehaviour, IGameStateListener
{
    public static UIManager Instance;
    public static Action<Panel> OnPanelShown;

    [Header("PANELS:")]
    [SerializeField] private GameObject usernamePanel;
    [SerializeField] private GameObject introPanel;
    [SerializeField] private GameObject menuPanel;
    [SerializeField] private GameObject weaponSelectPanel;
    [SerializeField] private GameObject gamePanel;
    [SerializeField] private GameObject gameoverPanel;
    [SerializeField] private GameObject waveBasedCompletePanel;
    [SerializeField] private GameObject waveTransitionPanel;
    [SerializeField] private GameObject traitSelectTransitionPanel;
    [SerializeField] private GameObject progressionPanel;
    [SerializeField] private GameObject shopPanel;
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject confirmationPanel;
    [SerializeField] private GameObject characterSelectPanel;
    [SerializeField] private GameObject statisticsPanel;
    [SerializeField] private GameObject settingPanel;
    [SerializeField] private GameObject codexPanel;
    [SerializeField] private GameObject missionPanel;
    [SerializeField] private GameObject equipmentPanel;
    [SerializeField] private GameObject progressionTreePanel;

    [Header("ADD. OBJECTS:")]
    [SerializeField] private List<GameObject> blockers = new();

    [Header("COUNTER TEXT:")]
    [SerializeField] private TextMeshProUGUI killCounterText;
    [SerializeField] private TextMeshProUGUI gameoverKillCounterText;
    [SerializeField] private TextMeshProUGUI levelUpText;
    [SerializeField] private TextMeshProUGUI wavesCompletedUpText;
    [SerializeField] private TextMeshProUGUI meatCollectedText;


    private List<GameObject> panels = new List<GameObject>();

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        panels.AddRange(new GameObject[]
        {
            usernamePanel,
            menuPanel,
            weaponSelectPanel,
            gamePanel,
            gameoverPanel,
            waveBasedCompletePanel,
            waveTransitionPanel,
            traitSelectTransitionPanel,
            progressionPanel,
            shopPanel
        });

        GameManager.OnGamePaused += PauseGameCallback;
        GameManager.OnGameResumed += ResumeGameCallback;

        pausePanel.SetActive(false);
        statisticsPanel.SetActive(false);   
        codexPanel.SetActive(false);

        HideConfirmationPanel();
        HideCharacterSelectPanel();
        HideSettingsPanel();
        HideMissionPanel();
        HideAllBlockers();
        HideEquipmentSelectPanel();
        HideProgressionTreeSelectPanel();

        CheckFirstTimeLoad();
    }

    private void Update() => UpdateCounterText();

    private void OnDestroy() 
    {
        GameManager.OnGamePaused -= PauseGameCallback;
        GameManager.OnGameResumed -= ResumeGameCallback;
    }

    public void GameStateChangedCallback(GameState _gameState)
    {
        switch(_gameState)
        {
            case GameState.Username:
                ShowPanel(usernamePanel);
                break;
            case GameState.Menu:
                ShowPanel(menuPanel);
                break;
            case GameState.WeaponSelect:
                ShowPanel(weaponSelectPanel);
                break;
            case GameState.Game:
                ShowPanel(gamePanel);
                break;
            case GameState.GameOver:
                ShowPanel(gameoverPanel);
                break;
            case GameState.StageCompleted:
                ShowPanel(waveBasedCompletePanel);
                break;
            case GameState.WaveTransition:
                ShowPanel(waveTransitionPanel);
                break;
            case GameState.TraitSelection:
                ShowPanel(traitSelectTransitionPanel);
                break;
            case GameState.Shop:
                ShowPanel(shopPanel);
                break;
        }
    }

    private void ShowPanel(GameObject panel, bool _hidePreviousPanels = true)
    {
        if (_hidePreviousPanels)
        {
            foreach (GameObject p in panels)
            {
                p.SetActive(p == panel);
                if (p == panel)
                {
                    TriggerPanelAction(panel);
                    CheckPanelTutorial(panel);
                }
            }
        }
        else
        {
            panel.SetActive(true);
            CheckPanelTutorial(panel);
        }
    }

    private void CheckPanelTutorial(GameObject panel)
    {
        if (panel.TryGetComponent<Panel>(out Panel panelComponent))
        {
            TutorialManager.Instance.CheckForTutorial(panelComponent.PanelId);
        }
    }

    private void UpdateCounterText() => killCounterText.text = StatisticsManager.Instance.CurrentRunKills.ToString();

    private void PauseGameCallback()
    {
        AudioManager.Instance.DecreaseMusicVolume();
        pausePanel.SetActive(true);
        TriggerPanelAction(pausePanel);
    }

    private void ResumeGameCallback()
    {
        AudioManager.Instance.ResetMusicVolume();   
        pausePanel.SetActive(false);
    }

    public void ShowConfirmationPanel()
    {
        confirmationPanel.SetActive(true);
        TriggerPanelAction(confirmationPanel);
        ShowPanelInteractability(pausePanel, false);
    }

    public void HideConfirmationPanel()
    {
        confirmationPanel.SetActive(false);
        TriggerPanelAction(pausePanel);
        ShowPanelInteractability(pausePanel, true);
    }

    public void ShowCharacterSelectPanel()
    {
        characterSelectPanel.SetActive(true);
        TriggerPanelAction(characterSelectPanel);
        menuPanel.SetActive(false);
    }

    public void HideCharacterSelectPanel()
    {
        characterSelectPanel.SetActive(false);
        menuPanel.SetActive(true);
        TriggerPanelAction(menuPanel);
    }

    public void ShowStatisticsPanel()
    {
        statisticsPanel.SetActive(true);
        TriggerPanelAction(statisticsPanel);
        menuPanel.SetActive(false);
    }

    public void HideStatisticsPanel()
    {
        statisticsPanel.SetActive(false);
        menuPanel.SetActive(true);
        TriggerPanelAction(menuPanel);
    }

    public void ShowSettingsPanel()
    {
        settingPanel.SetActive(true);
        TriggerPanelAction(settingPanel);
        menuPanel.SetActive(false);
    }

    public void HideSettingsPanel()
    {
        settingPanel.SetActive(false);
        menuPanel.SetActive(true);
        TriggerPanelAction(menuPanel);
    }

    public void ShowCodexPanel()
    {
        codexPanel.SetActive(true);
        TriggerPanelAction(codexPanel);
        menuPanel.SetActive(false);
    }

    public void HideCodexPanel()
    {
        codexPanel.SetActive(false);
        menuPanel.SetActive(true);
        TriggerPanelAction(menuPanel);
    }

    public void ShowMissionPanel()
    {
        missionPanel.SetActive(true);
        TriggerPanelAction(missionPanel);
        menuPanel.SetActive(false);
    }

    public void HideMissionPanel()
    {
        missionPanel.SetActive(false);
        menuPanel.SetActive(true);
        TriggerPanelAction(menuPanel);
    }

    public void ShowEquipmentSelectPanel()
    {
        equipmentPanel.SetActive(true);
        TriggerPanelAction(equipmentPanel);
        menuPanel.SetActive(false);
    }

    public void HideEquipmentSelectPanel()
    {
        equipmentPanel.SetActive(false);
        menuPanel.SetActive(true);
        TriggerPanelAction(equipmentPanel);
    }

    public void ShowProgressionTreeSelectPanel()
    {
        progressionTreePanel.SetActive(true);
        TriggerPanelAction(progressionTreePanel);
        menuPanel.SetActive(false);
    }

    public void HideProgressionTreeSelectPanel()
    {
        progressionTreePanel.SetActive(false);
        menuPanel.SetActive(true);
        TriggerPanelAction(progressionTreePanel);
    }


    public void ShowCharacterProgressionPanel()
    {
        TriggerPanelAction(progressionPanel);
        progressionPanel.SetActive(true);
        ProgressionManager.Instance.inGameProgressionUI.Refresh();
    }

    public void HideCharacterProgressionPanel()
    {
        TriggerPanelAction(progressionPanel);
        progressionPanel.SetActive(false);
    }

    private void TriggerPanelAction(GameObject _panelObject)
    {
        if (_panelObject.TryGetComponent(out Panel panelComponent))
            OnPanelShown?.Invoke(panelComponent);
    }

    public static void ShowPanelInteractability(GameObject _gameObject, bool _interactable)
    {
        if (_gameObject.TryGetComponent(out CanvasGroup cg))
            cg.interactable = _interactable;
    }

    #region Traits

    public void ShowBlockersUpTo(int blockerIndex)
    {
        for (int i = 0; i < blockers.Count; i++)
        {
            blockers[i].SetActive(i < blockerIndex);
        }
    }

    public void HideAllBlockers()
    {
        foreach (var blocker in blockers)
            blocker.SetActive(false);
    }

    #endregion

    #region Intro Handling

    private void CheckFirstTimeLoad()
    {
        bool introPlayed = false;
        SaveManager.TryLoad(this, "IntroPlayed", out object introPlayedObj);
        if (introPlayedObj is bool b && b)
            introPlayed = true;
    
        bool hasUsername = !UserManager.Instance.IsFirstTimePlayer();
    
        if (!introPlayed)
        {
            introPanel.SetActive(true);
            menuPanel.SetActive(false);
            usernamePanel.SetActive(false);
        }
        else if (!hasUsername)
        {
            introPanel.SetActive(false);
            usernamePanel.SetActive(true);
            menuPanel.SetActive(false);
        }
        else
        {
            introPanel.SetActive(false);
            usernamePanel.SetActive(false);
            menuPanel.SetActive(true);
        }
    }

    public void CompleteIntro()
    {
        SaveManager.Save(this, "IntroPlayed", true);
        introPanel.SetActive(false);
        menuPanel.SetActive(true);
    }

    public void UpdateGameoverPanel()
    {
        gameoverKillCounterText.text = StatisticsManager.Instance.CurrentRunKills.ToString();
        levelUpText.text = StatisticsManager.Instance.CurrentLevelUp.ToString();
        wavesCompletedUpText.text = StatisticsManager.Instance.CurrentWaveCompleted.ToString();
        meatCollectedText.text = StatisticsManager.Instance.CurrentMeatCollected.ToString();    

        if (gameoverPanel.TryGetComponent<GameOverStatsUI>(out var statsUI))
        {
            statsUI.UpdateStats();
        }
    }

    public void UpdateStageCompletionPanel()
    {
        if (waveBasedCompletePanel.TryGetComponent<StageCompletionStatsUI>(out var statsUI))
        {
            statsUI.UpdateStats();
        }
    }

    #endregion
}