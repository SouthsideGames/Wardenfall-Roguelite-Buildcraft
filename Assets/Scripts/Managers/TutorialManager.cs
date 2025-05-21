using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class TutorialManager : MonoBehaviour, IGameStateListener
{
    public static TutorialManager Instance { get; private set; }

    public GameObject tutorialPrefab;
    [SerializeField] private TutorialDataSO[] tutorials;

    private Dictionary<GameState, string> stateToTutorialMap;
    private Dictionary<string, bool> completedTutorials;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeStateTutorialMap();
            LoadCompletedTutorials();
        }
        else
            Destroy(gameObject);
    }

    private void InitializeStateTutorialMap()
    {
        stateToTutorialMap = new Dictionary<GameState, string>
        {
            { GameState.WeaponSelect, "tutorial_weapon_select" },
            { GameState.Game, "tutorial_gameplay" },
            { GameState.CardDraft, "tutorial_card_draft" },
            { GameState.TraitSelection, "tutorial_traits" },
            { GameState.Shop, "tutorial_shop" }
        };

        completedTutorials = new Dictionary<string, bool>();
    }

    private void LoadCompletedTutorials()
    {
        foreach (var tutorial in tutorials)
        {
            string key = $"Tutorial_{tutorial.panelId}";
            completedTutorials[tutorial.panelId] = PlayerPrefs.GetInt(key, 0) == 1;
        }
    }

    public void GameStateChangedCallback(GameState newState)
    {
        if (stateToTutorialMap.TryGetValue(newState, out string tutorialId))
            CheckForTutorial(tutorialId);
    }

    public void CheckForTutorial(string panelId)
    {
        if (completedTutorials.ContainsKey(panelId) && completedTutorials[panelId])
            return;

        GameState currentState = GameManager.Instance.gameState;
        
        if (currentState != GameState.Menu)
            return;

        bool isFirstTime = !PlayerPrefs.HasKey("HasPlayedBefore");
        if (!isFirstTime)
            return;

        foreach (var tutorial in tutorials)
        {
            if (tutorial.panelId == panelId)
            {
                ShowTutorial(tutorial);
                PlayerPrefs.SetInt("HasPlayedBefore", 1);
                PlayerPrefs.Save();
                break;
            }
        }
    }
    private void ShowTutorial(TutorialDataSO tutorial)
    {
        GameObject tutorialInstance = Instantiate(tutorialPrefab, UIManager.Instance.mainCanvas.transform);
        tutorialInstance.GetComponent<TutorialPrefabUI>().Initialize(tutorial);
    }

    public void CompleteTutorial(string panelId)
    {
        string key = $"Tutorial_{panelId}";
        PlayerPrefs.SetInt(key, 1);
        completedTutorials[panelId] = true;
    }
}