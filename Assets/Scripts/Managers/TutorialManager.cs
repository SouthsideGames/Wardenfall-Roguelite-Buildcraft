using UnityEngine;
using System.Collections.Generic;

public class TutorialManager : MonoBehaviour, IGameStateListener
{
    public static TutorialManager Instance { get; private set; }

    public GameObject tutorialPrefab;
    [SerializeField] private TutorialDataSO[] tutorials;

    private Dictionary<GameState, TutorialDataSO> stateToTutorialMap;
    private HashSet<TutorialDataSO> completedTutorials;

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
        stateToTutorialMap = new Dictionary<GameState, TutorialDataSO>();
        completedTutorials = new HashSet<TutorialDataSO>();

        foreach (var tutorial in tutorials)
        {
            // You can assign tutorials manually here
            // Or expand TutorialDataSO to include a GameState reference
            // Example:
            // if (tutorial.relatedGameState == GameState.WeaponSelect) stateToTutorialMap[GameState.WeaponSelect] = tutorial;
        }
    }

    private void LoadCompletedTutorials()
    {
        completedTutorials = new HashSet<TutorialDataSO>();

        foreach (var tutorial in tutorials)
        {
            string key = $"Tutorial_{tutorial.name}";
            if (PlayerPrefs.GetInt(key, 0) == 1)
                completedTutorials.Add(tutorial);
        }
    }

    public void GameStateChangedCallback(GameState newState)
    {
        if (stateToTutorialMap.TryGetValue(newState, out TutorialDataSO tutorial))
        {
            CheckForTutorial(tutorial);
        }
    }

    public void CheckForTutorial(TutorialDataSO tutorial)
    {
        if (completedTutorials.Contains(tutorial))
            return;

        ShowTutorial(tutorial);
        PlayerPrefs.SetInt($"Tutorial_{tutorial.name}", 1);
        PlayerPrefs.Save();
        completedTutorials.Add(tutorial);
    }

    private void ShowTutorial(TutorialDataSO tutorial)
    {
        GameObject tutorialInstance = Instantiate(tutorialPrefab, UIManager.Instance.mainCanvas.transform);
        var tutorialUI = tutorialInstance.GetComponent<TutorialPrefabUI>();
        if (tutorialUI != null)
            tutorialUI.Initialize(tutorial);
        else
            Debug.LogError("TutorialPrefabUI component missing on tutorialPrefab.");
    }

    public void CompleteTutorial(TutorialDataSO tutorial)
    {
        PlayerPrefs.SetInt($"Tutorial_{tutorial.name}", 1);
        completedTutorials.Add(tutorial);
    }
}
