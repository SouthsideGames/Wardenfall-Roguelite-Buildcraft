using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TutorialPrefabUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private Button nextButton;

    private TutorialDataSO currentTutorial;
    private int currentDialogueIndex;

    private void Awake()
    {
        nextButton.onClick.AddListener(OnNextButtonClicked);
    }

    public void Initialize(TutorialDataSO tutorialData)
    {
        currentTutorial = tutorialData;
        currentDialogueIndex = 0;
        UpdateDialogueText();
    }

    private void UpdateDialogueText()
    {
        if (currentDialogueIndex < currentTutorial.dialogueLines.Length)
        {
            dialogueText.text = currentTutorial.dialogueLines[currentDialogueIndex];
        }
    }

     private void OnNextButtonClicked()
    {
        currentDialogueIndex++;

        if (currentDialogueIndex >= currentTutorial.dialogueLines.Length)
        {
            TutorialManager.Instance.CompleteTutorial(currentTutorial.panelId);
            
            if (currentTutorial.panelId.StartsWith("main_menu_tutorial_"))
            {
                UIManager.Instance.ShowNextTutorialStep();
            }
            
            Destroy(gameObject);
        }
        else
        {
            UpdateDialogueText();
        }
    }
}
