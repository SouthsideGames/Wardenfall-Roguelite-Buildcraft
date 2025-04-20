
using UnityEngine;
using TMPro;
using DG.Tweening;
using System.Collections.Generic;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance { get; private set; }

    [Header("UI References")]
    [SerializeField] private GameObject tutorialPrefab;
    [SerializeField] private TextMeshProUGUI tutorialText;
    [SerializeField] private GameObject darkBackground;

    [Header("Tutorial Data")]
    [SerializeField] private TutorialDataSO[] tutorials;
    
    private HashSet<string> completedTutorials = new HashSet<string>();
    private int currentDialogueIndex = 0;
    private TutorialDataSO currentTutorial;
    private GameObject currentTutorialInstance;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        LoadCompletedTutorials();
    }

    public void CheckForTutorial(string panelId)
    {
        if (completedTutorials.Contains(panelId)) return;

        foreach (var tutorial in tutorials)
        {
            if (tutorial.panelId == panelId)
            {
                StartTutorial(tutorial);
                break;
            }
        }
    }

    private void StartTutorial(TutorialDataSO tutorial)
    {
        currentTutorial = tutorial;
        currentDialogueIndex = 0;
        
        if (currentTutorialInstance != null)
            Destroy(currentTutorialInstance);
            
        currentTutorialInstance = Instantiate(tutorialPrefab);
        tutorialText = currentTutorialInstance.GetComponentInChildren<TextMeshProUGUI>();
        darkBackground = currentTutorialInstance.transform.Find("DarkBackground").gameObject;
        
        ShowCurrentDialogue();
    }

    public void AdvanceDialogue()
    {
        currentDialogueIndex++;
        
        if (currentDialogueIndex >= currentTutorial.dialogueLines.Length)
        {
            CompleteTutorial();
            return;
        }
        
        ShowCurrentDialogue();
    }

    private void ShowCurrentDialogue()
    {
        tutorialText.text = currentTutorial.dialogueLines[currentDialogueIndex];
        tutorialText.transform.DOScale(1.1f, 0.3f).SetEase(Ease.OutBack);
    }

    private void CompleteTutorial()
    {
        completedTutorials.Add(currentTutorial.panelId);
        SaveCompletedTutorials();
        
        if (currentTutorialInstance != null)
            Destroy(currentTutorialInstance);
    }

    private void SaveCompletedTutorials()
    {
        string tutorialString = string.Join(",", completedTutorials);
        PlayerPrefs.SetString("CompletedTutorials", tutorialString);
    }

    private void LoadCompletedTutorials()
    {
        string tutorialString = PlayerPrefs.GetString("CompletedTutorials", "");
        if (!string.IsNullOrEmpty(tutorialString))
        {
            string[] tutorials = tutorialString.Split(',');
            completedTutorials = new HashSet<string>(tutorials);
        }
    }
}
