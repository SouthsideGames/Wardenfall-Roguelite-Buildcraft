using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections;
using System.Collections.Generic;

public class ImageTutorialPrefabUI : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Image slideImage;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private CanvasGroup slideCanvasGroup;
    [SerializeField] private Button nextButton;

    // Data
    private List<TutorialSlideData> slides;
    private int currentSlideIndex;
    private int currentDialogueIndex;

    private Action onComplete;

    private void Awake()
    {
        nextButton.onClick.AddListener(OnNextButtonClicked);
    }

    public void InitializeSlides(List<TutorialSlideData> slideDataList, Action onCompleteCallback = null)
    {
        slides = slideDataList;
        currentSlideIndex = 0;
        currentDialogueIndex = 0;
        onComplete = onCompleteCallback;

        slideCanvasGroup.alpha = 1f;

        ShowCurrentLine();
    }

    private void ShowCurrentLine()
    {
        if (currentSlideIndex >= slides.Count)
        {
            // No more slides - tutorial complete
            onComplete?.Invoke();
            Destroy(gameObject);
            return;
        }

        var currentSlide = slides[currentSlideIndex];

        if (currentDialogueIndex >= currentSlide.dialogueLines.Length)
            currentDialogueIndex = 0;

        slideImage.sprite = currentSlide.slideImage;

        dialogueText.text = currentSlide.dialogueLines[currentDialogueIndex];
    }

    private void OnNextButtonClicked()
    {
        var currentSlide = slides[currentSlideIndex];

        currentDialogueIndex++;

        if (currentDialogueIndex < currentSlide.dialogueLines.Length)
            ShowCurrentLine();
        else
        {
            currentSlideIndex++;
            currentDialogueIndex = 0;

            if (currentSlideIndex >= slides.Count)
            {
                onComplete?.Invoke();
                 GameManager.Instance.ResumeButtonCallback();
                Destroy(gameObject);
            }
            else
                StartCoroutine(FadeToNextSlide());
        }
    }

    private IEnumerator FadeToNextSlide()
    {
        yield return StartCoroutine(FadeCanvasGroup(1f, 0f, 0.3f));

        ShowCurrentLine();

        yield return StartCoroutine(FadeCanvasGroup(0f, 1f, 0.3f));
    }

    private IEnumerator FadeCanvasGroup(float start, float end, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            slideCanvasGroup.alpha = Mathf.Lerp(start, end, t);
            yield return null;
        }
        slideCanvasGroup.alpha = end;
    }
}
