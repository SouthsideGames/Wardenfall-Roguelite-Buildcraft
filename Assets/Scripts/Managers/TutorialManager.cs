
using UnityEngine;
using TMPro;
using DG.Tweening;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance { get; private set; }

    [Header("UI References")]
    [SerializeField] private GameObject tutorialPanel;
    [SerializeField] private TextMeshProUGUI tutorialText;
    [SerializeField] private GameObject highlightObject;
    [SerializeField] private GameObject arrowIndicator;

    private int currentStep = 0;
    private bool tutorialComplete = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void StartTutorial()
    {
        if (PlayerPrefs.GetInt("TutorialComplete", 0) == 1) return;
        currentStep = 0;
        ShowNextTutorialStep();
    }

    private void ShowNextTutorialStep()
    {
        switch (currentStep)
        {
            case 0: // Movement
                ShowTutorial("Use WASD or joystick to move your character");
                break;

            case 1: // Combat
                ShowTutorial("Click or tap to attack enemies");
                break;

            case 2: // Items
                ShowTutorial("Collect items by walking over them");
                break;

            case 3: // Cards
                ShowTutorial("Cards provide powerful abilities. Select one when offered!");
                break;

            case 4: // Traits
                ShowTutorial("Traits modify your character. Choose wisely!");
                break;

            case 5: // Weapons
                ShowTutorial("Find and upgrade weapons to become stronger");
                break;

            case 6: // Wave System
                ShowTutorial("Survive waves of enemies to progress");
                break;

            default:
                CompleteTutorial();
                break;
        }
    }

    public void AdvanceTutorial()
    {
        currentStep++;
        ShowNextTutorialStep();
    }

    private void ShowTutorial(string message)
    {
        tutorialPanel.SetActive(true);
        tutorialText.text = message;
        
        // Animate the panel
        tutorialPanel.transform.DOScale(1.1f, 0.3f).SetEase(Ease.OutBack);
    }

    private void CompleteTutorial()
    {
        tutorialPanel.SetActive(false);
        PlayerPrefs.SetInt("TutorialComplete", 1);
        tutorialComplete = true;
    }

    public void ShowHighlight(Vector3 position)
    {
        highlightObject.transform.position = position;
        highlightObject.SetActive(true);
    }

    public void HideHighlight()
    {
        highlightObject.SetActive(false);
    }

    public bool IsTutorialComplete() => tutorialComplete;
}
