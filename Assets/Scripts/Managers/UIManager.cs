using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System;

public class UIManager : MonoBehaviour, IGameStateListener
{
    public static Action<Panel> OnPanelShown;

    [Header("PANELS:")]
    [SerializeField] private GameObject menuPanel;
    [SerializeField] private GameObject weaponSelectPanel;
    [SerializeField] private GameObject gameModeSelectPanel;
    [SerializeField] private GameObject gamePanel;
    [SerializeField] private GameObject gameoverPanel;
    [SerializeField] private GameObject waveBasedCompletePanel;
    [SerializeField] private GameObject survivalCompletePanel;
    [SerializeField] private GameObject waveTransitionPanel;
    [SerializeField] private GameObject shopPanel;
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject confirmationPanel;
    [SerializeField] private GameObject characterSelectPanel;
    [SerializeField] private GameObject statisticsPanel;
    [SerializeField] private GameObject settingPanel;
    [SerializeField] private GameObject codexPanel;
    [SerializeField] private GameObject missionPanel;
    [SerializeField] private GameObject loadoutPanel;
    [SerializeField] private GameObject menuShopPanel;

    [Header("REFERENCES")]
    [SerializeField] private CardManager cardManager;
    [SerializeField] private MainMenuShopUpdateUI mainMenuShopManagerUI;

    [Header("COUNTER TEXT:")]
    [SerializeField] private TextMeshProUGUI killCounterText;

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
            waveBasedCompletePanel,
            survivalCompletePanel,
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
        HideDeckbuilderPanel();
        HideMenuShopPanel();
    
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
                ShowPanel(waveBasedCompletePanel);
                break;
            case GameState.SurvivalStageCompleted:
                ShowPanel(survivalCompletePanel);
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

                if (p == panel)
                   TriggerPanelAction(panel);
            }
        }
        else
            panel.SetActive(true);
       
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

    public void ShowDeckbuilderPanel()
    {
        loadoutPanel.SetActive(true);
        TriggerPanelAction(loadoutPanel);
        menuPanel.SetActive(false);
    }
    public void HideDeckbuilderPanel()
    {
        loadoutPanel.SetActive(false);
        cardManager.SpawnInGameCards();
        menuPanel.SetActive(true);
        TriggerPanelAction(menuPanel);
    }

    public void ShowMenuShopPanel()
    {
        menuShopPanel.SetActive(true);
        menuPanel.SetActive(false);
        
    }

    public void HideMenuShopPanel()
    {
        menuShopPanel.SetActive(false);
        menuPanel.SetActive(true);
    }

    public void GemMenuShopPanel()
    {
        menuShopPanel.SetActive(true);
        mainMenuShopManagerUI.OpenGemContainer();
    }

    public void CashMenuShopPanel()
    {
        menuShopPanel.SetActive(true);
        mainMenuShopManagerUI.OpenCashContainer();
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

}
