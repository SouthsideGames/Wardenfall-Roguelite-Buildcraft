using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour, IGameStateListener
{
    [Header("PANELS:")]
    [SerializeField] private GameObject menuPanel;
    [SerializeField] private GameObject weaponSelectPanel;
    [SerializeField] private GameObject gamePanel;
    [SerializeField] private GameObject gameoverPanel;
    [SerializeField] private GameObject stageCompletePanel;
    [SerializeField] private GameObject waveTransitionPanel;
    [SerializeField] private GameObject shopPanel;
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject confirmationPanel;


    private List<GameObject> panels = new List<GameObject>();

    private void Awake()
    {
        panels.AddRange(new GameObject[]
        {
            menuPanel,
            weaponSelectPanel,
            gamePanel,
            gameoverPanel,
            stageCompletePanel,
            waveTransitionPanel,
            shopPanel

        });

        GameManager.onGamePaused += PauseGameCallback;
        GameManager.onGameResumed += ResumeGameCallback;

        pausePanel.SetActive(false);
        HideConfirmationPanel();
    
    }

    private void OnDestroy() 
    {
        GameManager.onGamePaused -= PauseGameCallback;
        GameManager.onGameResumed -= ResumeGameCallback;
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

    private void PauseGameCallback() => pausePanel.SetActive(true);
    private void ResumeGameCallback() =>  pausePanel.SetActive(false);
    public void ShowConfirmationPanel() =>   confirmationPanel.SetActive(true);
    public void HideConfirmationPanel() => confirmationPanel.SetActive(false);
}
