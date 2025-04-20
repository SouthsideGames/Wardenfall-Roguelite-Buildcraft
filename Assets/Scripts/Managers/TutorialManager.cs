
using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class TutorialManager : MonoBehaviour, IGameStateListener
{
    public static TutorialManager Instance { get; private set; }

    [Header("UI References")]
    [SerializeField] private GameObject tutorialPrefab;
    [SerializeField] private GameObject darkBackground;
    [SerializeField] private TextMeshProUGUI tutorialText;

    [Header("Tutorial Data")]
    [SerializeField] private TutorialDataSO[] tutorials;
    
    private Dictionary<GameState, string> stateToTutorialMap;
    private Dictionary<string, bool> completedTutorials;
    private GameObject currentTutorialInstance;
    private int currentDialogueIndex;

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
        {
            Destroy(gameObject);
        }
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
        {
            CheckForTutorial(tutorialId);
        }
    }

    public void CheckForTutorial(string panelId)
    {
        if (completedTutorials.ContainsKey(panelId) && completedTutorials[panelId])
            return;

        foreach (var tutorial in tutorials)
        {
            if (tutorial.panelId == panelId)
            {
                ShowTutorial(tutorial);
                break;
            }
        }
    }

    private void ShowTutorial(TutorialDataSO tutorial)
    {
        if (currentTutorialInstance != null)
            Destroy(currentTutorialInstance);

        currentTutorialInstance = Instantiate(tutorialPrefab);
        currentDialogueIndex = 0;
        
        UpdateTutorialText(tutorial);
        darkBackground.SetActive(true);
    }

    private void UpdateTutorialText(TutorialDataSO tutorial)
    {
        if (currentDialogueIndex < tutorial.dialogueLines.Length)
        {
            tutorialText.text = tutorial.dialogueLines[currentDialogueIndex];
        }
    }

    public void OnTutorialNext()
    {
        var currentTutorial = GetCurrentTutorial();
        if (currentTutorial == null) return;

        currentDialogueIndex++;
        
        if (currentDialogueIndex >= currentTutorial.dialogueLines.Length)
        {
            CompleteTutorial(currentTutorial);
        }
        else
        {
            UpdateTutorialText(currentTutorial);
        }
    }

    private TutorialDataSO GetCurrentTutorial()
    {
        return tutorials.Length > 0 ? tutorials[0] : null;
    }

    private void CompleteTutorial(TutorialDataSO tutorial)
    {
        string key = $"Tutorial_{tutorial.panelId}";
        PlayerPrefs.SetInt(key, 1);
        completedTutorials[tutorial.panelId] = true;
        
        if (currentTutorialInstance != null)
        {
            Destroy(currentTutorialInstance);
            currentTutorialInstance = null;
        }
        
        darkBackground.SetActive(false);
    }
}
