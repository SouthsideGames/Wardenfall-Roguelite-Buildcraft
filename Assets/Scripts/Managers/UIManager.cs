using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System;
using SouthsideGames.SaveManager;
using UnityEngine.UI;

public class UIManager : MonoBehaviour, IGameStateListener
{
    public static UIManager Instance;
    public Transform mainCanvas;


    [Header("TUTORIAL:")]
    [SerializeField] private GameObject[] mainMenuButtons;
    private int currentTutorialStep = 0;
    [SerializeField] private TutorialDataSO newGameTutorialData;

    [Header("PANELS:")]
    [SerializeField] private GameObject usernamePanel;
    [SerializeField] private GameObject introPanel;
    [SerializeField] private GameObject menuPanel;
    [SerializeField] private GameObject weaponSelectPanel;
    [SerializeField] private GameObject gamePanel;
    [SerializeField] private GameObject gameoverPanel;
    [SerializeField] private GameObject stageCompletePanel;
    [SerializeField] private GameObject waveTransitionPanel;
    [SerializeField] private GameObject traitSelectTransitionPanel;
    [SerializeField] private GameObject progressionPanel;
    [SerializeField] private GameObject progressionInfoPanel;
    [SerializeField] private GameObject shopPanel;
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject notificationPanel;
    [SerializeField] private GameObject confirmationPanel;
    [SerializeField] private GameObject characterSelectPanel;
    [SerializeField] private GameObject statisticsPanel;
    [SerializeField] private GameObject settingPanel;
    [SerializeField] private GameObject codexPanel;
    [SerializeField] private GameObject missionPanel;
    [SerializeField] private GameObject equipmentPanel;
    [SerializeField] private GameObject progressionTreePanel;
    [SerializeField] private GameObject challengePanel;
    [SerializeField] private GameObject challengeInfoPanel;
    [SerializeField] private GameObject gearroomPanel;
    [SerializeField] private GameObject leaderboardPanel;

    [Header("ADD. OBJECTS:")]
    [SerializeField] private List<CanvasGroup> blockers = new();
    [SerializeField] private AudioClip introMusic;
    public ViewerRatingUI viewerRatingSlider;
    [SerializeField] private TextMeshProUGUI challengeStatusText;
    public GameObject toastPrefab;

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
            progressionPanel,
            stageCompletePanel,
            waveTransitionPanel,
            traitSelectTransitionPanel,
            progressionPanel,
            shopPanel,
            challengePanel,
        });

        GameManager.OnGamePaused += PauseGameCallback;
        GameManager.OnGameResumed += ResumeGameCallback;

        pausePanel.SetActive(false);

        ShowBlockersUpTo(3);

        HideAllPanels();

    }


    private void Start()
    {
        CheckFirstTimeLoad();
    }

    private void Update() => UpdateCounterText();

    private void OnDestroy()
    {
        GameManager.OnGamePaused -= PauseGameCallback;
        GameManager.OnGameResumed -= ResumeGameCallback;
    }

    private void ShowPanel(GameObject panel, bool _hidePreviousPanels = true)
    {
        if (_hidePreviousPanels)
        {
            foreach (GameObject p in panels)
            {
                p.SetActive(p == panel);
            }
        }
        else
        {
            panel.SetActive(true);
        }
    }


    private void UpdateCounterText() => killCounterText.text = StatisticsManager.Instance.CurrentRunKills.ToString();

    public static void ShowPanelInteractability(GameObject _gameObject, bool _interactable)
    {
        if (_gameObject.TryGetComponent(out CanvasGroup cg))
            cg.interactable = _interactable;
    }

    public void TransitionPanel(GameObject fromPanel, GameObject toPanel)
    {
        if (fromPanel != null) fromPanel.SetActive(false);
        if (toPanel != null) toPanel.SetActive(true);

    }

    #region Callback Functions
    public void GameStateChangedCallback(GameState _gameState)
    {
        switch (_gameState)
        {
            case GameState.Username:
                ShowPanel(usernamePanel);
                break;
            case GameState.Menu:
                ShowPanel(menuPanel);
                UpdateChallengeStatusUI();
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
            case GameState.Progression:
                ShowPanel(progressionPanel);
                ProgressionManager.Instance.progressionPanelUI.Refresh();
                break;
            case GameState.StageCompleted:
                ShowPanel(stageCompletePanel);
                break;
            case GameState.WaveTransition:
                ShowPanel(waveTransitionPanel);
                break;
            case GameState.TraitSelection:
                if (!ChallengeManager.IsActive(ChallengeMode.TraitChaos))
                    ShowPanel(traitSelectTransitionPanel);
                break;
            case GameState.Shop:
                ShowPanel(shopPanel);
                break;
        }
    }

    private void PauseGameCallback()
    {
        AudioManager.Instance.DecreaseMusicVolume();
        pausePanel.SetActive(true);
  
    }

    private void ResumeGameCallback()
    {
        AudioManager.Instance.ResetMusicVolume();
        pausePanel.SetActive(false);
    }

    #endregion

    #region Show/Hide Panels

    public void ShowConfirmationPanel()
    {
        confirmationPanel.SetActive(true);
      
        ShowPanelInteractability(pausePanel, false);
    }

    public void HideConfirmationPanel()
    {
        confirmationPanel.SetActive(false);
        
        ShowPanelInteractability(pausePanel, true);
    }

    public void ShowCharacterSelectPanel()
    {
        characterSelectPanel.SetActive(true);
   
        menuPanel.SetActive(false);
    }

    public void HideCharacterSelectPanel()
    {
        characterSelectPanel.SetActive(false);
        menuPanel.SetActive(true);
    
    }

    public void ShowStatisticsPanel()
    {
        statisticsPanel.SetActive(true);

        menuPanel.SetActive(false);
    }

    public void HideStatisticsPanel()
    {
        statisticsPanel.SetActive(false);
        menuPanel.SetActive(true);
    
    }

    public void ShowNotificationPanel()
    {
        notificationPanel.SetActive(true);

        menuPanel.SetActive(false);
    }

    public void HideNotificationPanel()
    {
        notificationPanel.SetActive(false);
        menuPanel.SetActive(true);
    
    }

    public void ShowSettingsPanel()
    {
        settingPanel.SetActive(true);

        menuPanel.SetActive(false);
    }

    public void HideSettingsPanel()
    {
        settingPanel.SetActive(false);
        menuPanel.SetActive(true);

    }

    public void ShowCodexPanel()
    {
        codexPanel.SetActive(true);

        menuPanel.SetActive(false);
    }

    public void HideCodexPanel()
    {
        codexPanel.SetActive(false);
        menuPanel.SetActive(true);
    }

    public void ShowGearRoomPanel()
    {
        gearroomPanel.SetActive(true);
        menuPanel.SetActive(false);
    }

    public void HideGearRoomPanel()
    {
        gearroomPanel.SetActive(false);
        menuPanel.SetActive(true);

    }

    public void ShowMissionPanel()
    {
        missionPanel.SetActive(true);

        StatisticsManager.Instance.TurnOnRecordButton();
        menuPanel.SetActive(false);
    }

    public void HideMissionPanel()
    {
        missionPanel.SetActive(false);
        menuPanel.SetActive(true);
    }

    public void ShowEquipmentSelectPanel()
    {
        equipmentPanel.SetActive(true);
        menuPanel.SetActive(false);
    }

    public void HideEquipmentSelectPanel()
    {
        equipmentPanel.SetActive(false);
        menuPanel.SetActive(true);
    }

    public void ShowCharacterProgressionPanel()
    {
        ProgressionManager.Instance.progressionPanelUI.Refresh();
    }

    public void HideCharacterProgressionPanel()
    {
        progressionPanel.SetActive(false);
        menuPanel.SetActive(true);
    }

    public void ShowProgressionInfoPanel()
    {
        progressionInfoPanel.SetActive(true);
        ShowPanelInteractability(progressionInfoPanel, false);
    }

    public void HideProgressionInfoPanel()
    {
        progressionInfoPanel.SetActive(false);
        ShowPanelInteractability(progressionInfoPanel, true);
    }

    public void ShowChallengePanel()
    {
        challengePanel.SetActive(true);
        menuPanel.SetActive(false);
    }

    public void HideChallengePanel()
    {
        challengePanel.SetActive(false);
        menuPanel.SetActive(true);
        UpdateChallengeStatusUI();
    }

    public void ShowChallengeInfoPanel()
    {
        challengeInfoPanel.SetActive(true);
        ShowPanelInteractability(challengeInfoPanel, false);
    }

    public void HideChallengeInfoPanel()
    {
        challengeInfoPanel.SetActive(false);
        ShowPanelInteractability(challengeInfoPanel, true);
    }

    public void ShowLeaderboardPanel()
    {
        leaderboardPanel.SetActive(true);
        PlayfabManager.Instance.GetHighScores();
        menuPanel.SetActive(false);
    }

    public void HideLeaderboardPanel()
    {
        leaderboardPanel.SetActive(false);
        menuPanel.SetActive(true);
    }

    private void HideAllPanels()
    {
        HideConfirmationPanel();
        HideCharacterSelectPanel();
        HideSettingsPanel();
        HideMissionPanel();
        HideEquipmentSelectPanel();
        HideChallengePanel();
        HideNotificationPanel();
        HideGearRoomPanel();
        HideCharacterProgressionPanel();
        HideCodexPanel();
        HideStatisticsPanel();
        HideLeaderboardPanel();
        HideProgressionInfoPanel();
    }

    #endregion

    #region Traits

    public void ShowBlockersUpTo(int blockerIndex)
    {
        for (int i = 0; i < blockers.Count; i++)
        {
            if (blockers[i].TryGetComponent(out CanvasGroup cg))
            {
                cg.alpha = i < blockerIndex ? 1f : 0f;
                cg.interactable = i < blockerIndex;
                cg.blocksRaycasts = i < blockerIndex;
            }
        }
    }


    public void HideAllBlockers()
    {
        foreach (var blocker in blockers)
        {
            if (blocker.TryGetComponent(out CanvasGroup cg))
            {
                cg.alpha = 0f;
                cg.interactable = false;
                cg.blocksRaycasts = false;
            }
        }
    }

    #endregion

    #region Intro Handling

    private void CheckFirstTimeLoad()
    {
        introPanel.SetActive(false);
        usernamePanel.SetActive(false);
        menuPanel.SetActive(false);

        bool introPlayed = false;
        SaveManager.TryLoad(this, "IntroPlayed", out object introPlayedObj);
        if (introPlayedObj is bool b && b)
            introPlayed = true;

        bool hasUsername = !UserManager.Instance.IsFirstTimePlayer();

        if (!introPlayed)
        {
            AudioManager.Instance.ChangeMusic(introMusic);
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
        usernamePanel.SetActive(true); // instead of menu
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
        if (stageCompletePanel.TryGetComponent<StageCompletionStatsUI>(out var statsUI))
        {
            statsUI.UpdateStats();
        }
    }

    public void ShowFirstTimeUI()
    {
        foreach (GameObject button in mainMenuButtons)
        {
            button.SetActive(false);
        }
    }

    public void CompleteTutorial()
    {
        foreach (GameObject button in mainMenuButtons)
        {
            button.SetActive(true);
        }
    }

    public void ShowNextTutorialStep()
    {
        if (currentTutorialStep >= newGameTutorialData.dialogueLines.Length)
        {
            CompleteTutorial();
            return;
        }

        GameObject tutorialInstance = Instantiate(TutorialManager.Instance.tutorialPrefab, mainCanvas.transform);
        RectTransform tutorialRect = tutorialInstance.GetComponent<RectTransform>();

        // Start tutorial off-screen
        tutorialRect.anchoredPosition = new Vector2(0, Screen.height);

        string currentLine = newGameTutorialData.dialogueLines[currentTutorialStep];
        tutorialInstance.GetComponent<TutorialPrefabUI>().InitializeSingleLine(currentLine);

        // Animate tutorial into view
        LeanTween.moveY(tutorialRect, 0, 0.5f)
            .setEaseOutBack()
            .setIgnoreTimeScale(true);

        if (currentTutorialStep > 0 && currentTutorialStep < mainMenuButtons.Length + 1)
        {
            GameObject button = mainMenuButtons[currentTutorialStep - 1];
            button.SetActive(true);

            // Animate button appearance
            RectTransform buttonRect = button.GetComponent<RectTransform>();
            Vector3 originalScale = buttonRect.localScale;
            buttonRect.localScale = Vector3.zero;

            LeanTween.scale(buttonRect, originalScale, 0.5f)
                .setEaseOutBack()
                .setDelay(0.3f)
                .setIgnoreTimeScale(true);
        }

        currentTutorialStep++;
    }

    public void SkipIntroAndGoTo(GameObject targetPanel)
    {
        SaveManager.Save(this, "IntroPlayed", true);
        TransitionPanel(introPanel, targetPanel);
    }

    private void UpdateChallengeStatusUI()
    {
        if (challengeStatusText == null) return;

        var mode = ChallengeManager.Instance?.GetCurrentChallenge() ?? ChallengeMode.None;

        if (mode == ChallengeMode.None)
            challengeStatusText.text = "";
        else
            challengeStatusText.text = $"Challenge Active: {mode}";

        // Fade in using LeanTween
        challengeStatusText.alpha = 0;
        LeanTween.value(challengeStatusText.gameObject, 0, 1, 0.5f)
            .setOnUpdate((float val) => challengeStatusText.alpha = val)
            .setIgnoreTimeScale(true);
    }
    
    public void ShowToast(string message)
    {
        GameObject toast = Instantiate(toastPrefab, mainCanvas);
        var text = toast.GetComponentInChildren<TextMeshProUGUI>();
        text.text = message;

        CanvasGroup cg = toast.GetComponent<CanvasGroup>();
        cg.alpha = 0;
        LeanTween.alphaCanvas(cg, 1f, 0.4f).setEaseOutExpo();
        LeanTween.alphaCanvas(cg, 0f, 0.4f).setDelay(2.5f).setEaseInExpo()
            .setOnComplete(() => Destroy(toast));
    }




    #endregion
}