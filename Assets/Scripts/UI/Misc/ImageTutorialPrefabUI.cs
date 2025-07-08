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

    private List<TutorialSlideData> slides;
    private int currentSlideIndex;
    private Action onComplete;

    private void Awake()
    {
        nextButton.onClick.AddListener(OnNextButtonClicked);
    }

    public void InitializeSlides(List<TutorialSlideData> slideDataList, Action onCompleteCallback = null)
    {
        slides = slideDataList;
        currentSlideIndex = 0;
        onComplete = onCompleteCallback;

        slideCanvasGroup.alpha = 1f;
        ShowSlide(currentSlideIndex);
    }

    private void ShowSlide(int index)
    {
        if (index < slides.Count)
        {
            slideImage.sprite = slides[index].slideImage;
            dialogueText.text = slides[index].dialogueLine;
        }
    }

    private void OnNextButtonClicked()
    {
        currentSlideIndex++;

        if (currentSlideIndex >= slides.Count)
        {
            onComplete?.Invoke();
            Destroy(gameObject);
        }
        else
        {
            StartCoroutine(FadeToNextSlide());
        }
    }

    private IEnumerator FadeToNextSlide()
    {
        yield return StartCoroutine(FadeCanvasGroup(1f, 0f, 0.3f));
        ShowSlide(currentSlideIndex);
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
