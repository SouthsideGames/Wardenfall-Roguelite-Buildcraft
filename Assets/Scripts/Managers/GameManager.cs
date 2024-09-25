using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

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
        SetGameState(GameState.MENU);
    }

    public void StartGame() => SetGameState(GameState.GAME);    
    public void StartWeaponSelect() => SetGameState(GameState.WEAPONSELECT);
    public void StartShop() => SetGameState(GameState.SHOP);    
    public void StartGameOver() => SetGameState(GameState.GAMEOVER);  

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
        if(CharacterManager.Instance.HasLeveledUp())
        {
            SetGameState(GameState.WAVETRANSITION);
        }
        else
        {
            SetGameState(GameState.SHOP);
        }
    }

    public void ManageGameOver()
    {
        SceneManager.LoadScene(0);
    }
}


