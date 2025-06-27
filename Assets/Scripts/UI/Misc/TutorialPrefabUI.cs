using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TutorialPrefabUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private Button nextButton;

    private TutorialDataSO currentTutorial;
    private int currentDialogueIndex;
    private bool isSingleLine;

    private void Awake() => nextButton.onClick.AddListener(OnNextButtonClicked);

    public void Initialize(TutorialDataSO tutorialData)
    {
        currentTutorial = tutorialData;
        currentDialogueIndex = 0;
        isSingleLine = false;
        UpdateDialogueText();
    }

    public void InitializeSingleLine(string line)
    {
        dialogueText.text = line;
        isSingleLine = true;
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
        if (isSingleLine)
        {
            UIManager.Instance.ShowNextTutorialStep();
            Destroy(gameObject);
            return;
        }

        currentDialogueIndex++;

        if (currentDialogueIndex >= currentTutorial.dialogueLines.Length)
        {
            // Mark this tutorial complete
            TutorialManager.Instance.CompleteTutorial(currentTutorial);

            UIManager.Instance.CompleteIntro();

            Destroy(gameObject);
        }
        else
        {
            UpdateDialogueText();
        }
    }

}
