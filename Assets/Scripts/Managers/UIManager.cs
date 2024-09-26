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
    }

    public void GameStateChangedCallback(GameState _gameState)
    {
        switch(_gameState)
        {
            case GameState.MENU:
                ShowPanel(menuPanel);
                break;
            case GameState.WEAPONSELECT:
                ShowPanel(weaponSelectPanel);
                break;
            case GameState.GAME:
                ShowPanel(gamePanel);
                break;
            case GameState.GAMEOVER:
                ShowPanel(gameoverPanel);
                break;
            case GameState.STAGECOMPLETED:
                ShowPanel(stageCompletePanel);
                break;
            case GameState.WAVETRANSITION:
                ShowPanel(waveTransitionPanel);
                break;
            case GameState.SHOP:
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
}
