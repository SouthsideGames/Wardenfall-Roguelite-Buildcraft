using UnityEngine;
using System.Collections.Generic;

public class TutorialManager : MonoBehaviour, IGameStateListener
{
    public static TutorialManager Instance { get; private set; }

    public GameObject tutorialPrefab;
    [SerializeField] private GameObject imageTutorialPrefab;
    [SerializeField] private TutorialDataSO[] tutorials;
    [SerializeField] private ImageTutorialDataSO imageTutorialData;
    [SerializeField] private Transform tutorialSpawnPoint;


    private Dictionary<GameState, TutorialDataSO> stateToTutorialMap;
    private HashSet<TutorialDataSO> completedTutorials;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        InitializeStateTutorialMap();
        LoadCompletedTutorials();
    }

    private void InitializeStateTutorialMap()
    {
        stateToTutorialMap = new Dictionary<GameState, TutorialDataSO>();
        completedTutorials = new HashSet<TutorialDataSO>();

        foreach (var tutorial in tutorials)
        {

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
    }

    public void CompleteTutorial(TutorialDataSO tutorial)
    {
        PlayerPrefs.SetInt($"Tutorial_{tutorial.name}", 1);
        completedTutorials.Add(tutorial);
    }

    public void CheckAndShowFirstTimeTutorial()
    {
        Debug.Log("[TutorialManager] CheckAndShowFirstTimeTutorial CALLED!");
        if (!UserManager.Instance.NeedsFirstTimeTutorial())
            return;

        ShowFirstTimeTutorial();
    }

    // First-time tutorial
    private void ShowFirstTimeTutorial()
    {
        Debug.Log("[TutorialManager] In ShowFirstTimeTutorial!");

        GameManager.Instance.PauseGame();

        Debug.Log($"Instantiating: {imageTutorialPrefab} under {tutorialSpawnPoint}");

        GameObject tutorialInstance = Instantiate(imageTutorialPrefab, tutorialSpawnPoint);

        if (tutorialInstance == null)
        {
            Debug.LogError("[TutorialManager] Instantiation FAILED!");
            return;
        }

        Debug.Log("[TutorialManager] Instantiated successfully!");

        var tutorialUI = tutorialInstance.GetComponent<ImageTutorialPrefabUI>();

        if (tutorialUI == null)
        {
            Debug.LogError("[TutorialManager] ImageTutorialPrefabUI component NOT found on prefab!");
            return;
        }

        Debug.Log("[TutorialManager] Initializing slides!");

        tutorialUI.InitializeSlides(new List<TutorialSlideData>(imageTutorialData.slides), OnFirstTimeTutorialComplete);
    }


   


    
    private void OnFirstTimeTutorialComplete()
    {
        UserManager.Instance.MarkFirstTimeTutorialSeen();
        GameManager.Instance.ResumeGame();
    }

}
