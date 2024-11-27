using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour, IGameStateListener
{
    [Header("PANELS:")]
    [SerializeField] private GameObject menuPanel;
    [SerializeField] private GameObject weaponSelectPanel;
    [SerializeField] private GameObject gameModeSelectPanel;
    [SerializeField] private GameObject gamePanel;
    [SerializeField] private GameObject gameoverPanel;
    [SerializeField] private GameObject stageCompletePanel;
    [SerializeField] private GameObject waveTransitionPanel;
    [SerializeField] private GameObject shopPanel;
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject confirmationPanel;
    [SerializeField] private GameObject characterSelectPanel;
    [SerializeField] private GameObject statisticsPanel;
    [SerializeField] private GameObject settingPanel;
    [SerializeField] private GameObject codexPanel;
    [SerializeField] private GameObject missionPanel;

    [Header("COUNTER TEXT:")]
    [SerializeField] private TextMeshProUGUI killCounterText;
    [SerializeField] private TextMeshProUGUI chestCounterText;

    private List<GameObject> panels = new List<GameObject>();

    private void Awake()
    {
        panels.AddRange(new GameObject[]
        {
            menuPanel,
            weaponSelectPanel,
            gameModeSelectPanel,
            gamePanel,
            gameoverPanel,
            stageCompletePanel,
            waveTransitionPanel,
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
            case GameState.Menu:
                ShowPanel(menuPanel);
                break;
            case GameState.WeaponSelect:
                ShowPanel(weaponSelectPanel);
                break;
            case GameState.GameModeSelect:
                ShowPanel(gameModeSelectPanel);
                break;
            case GameState.Game:
                ShowPanel(gamePanel);
                break;
            case GameState.GameOver:
                ShowPanel(gameoverPanel);
                break;
            case GameState.StageCompleted:
                ShowPanel(stageCompletePanel);
                break;
            case GameState.WaveTransition:
                ShowPanel(waveTransitionPanel);
                break;
            case GameState.Shop:
                ShowPanel(shopPanel);
                break;

        }
    }

    private void ShowPanel(GameObject panel, bool _hidePreviousPanels = true)
    {
        if(_hidePreviousPanels)
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

    private void UpdateCounterText()
    {
        killCounterText.text = StatisticsManager.Instance.CurrentRunKills.ToString();
        chestCounterText.text = StatisticsManager.Instance.CurrentChestCollected.ToString();
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
    
    public void ShowConfirmationPanel() =>   confirmationPanel.SetActive(true);
    public void HideConfirmationPanel() => confirmationPanel.SetActive(false);
    public void ShowCharacterSelectPanel() => characterSelectPanel.SetActive(true);
    public void HideCharacterSelectPanel() => characterSelectPanel.SetActive(false);   
    public void ShowStatisticsPanel() => statisticsPanel.SetActive(true);
    public void HideStatisticsPanel() => statisticsPanel.SetActive(false);   
    public void ShowSettingsPanel() => settingPanel.SetActive(true);
    public void HideSettingsPanel() => settingPanel.SetActive(false);  
    public void ShowCodexPanel() => codexPanel.SetActive(true);
    public void HideCodexPanel() => codexPanel.SetActive(false);  
    public void ShowMissionPanel() => missionPanel.SetActive(true);
    public void HideMissionPanel() => missionPanel.SetActive(false);  

}
