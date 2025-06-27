using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class CreditsScrollerUI : MonoBehaviour
{
    [Header("ELEMENTS:")]
    [SerializeField] private float scrollSpeed = 100f;
    [SerializeField] private float endYPosition = 1000f;
    private RectTransform rt;

    [Header("INTRO SETTINGS:")]
    [SerializeField] private bool isIntroScroller = false;
    [SerializeField] private TutorialDataSO introTutorialData;
    [SerializeField] private Transform tutorialSpawnPoint;
    [SerializeField] private GameObject tutorialPrefab;

    private bool hasFinished = false;

    private void Awake()
    {
        rt = GetComponent<RectTransform>();
    }

    private void OnEnable()
    {
        rt.anchoredPosition = rt.anchoredPosition.With(y: 0);
        hasFinished = false;
    }

    private void Update()
    {
        if (hasFinished)
            return;

        rt.anchoredPosition += Vector2.up * Time.deltaTime * scrollSpeed;

        if (isIntroScroller && rt.anchoredPosition.y >= endYPosition)
        {
            hasFinished = true;
            SpawnIntroTutorial();
        }
    }

    private void SpawnIntroTutorial()
    {
        if (tutorialPrefab == null || tutorialSpawnPoint == null)
        {
            Debug.LogWarning("TutorialPrefab or SpawnPoint not set on CreditsScrollerUI!");
            return;
        }

        GameObject tutorialInstance = Instantiate(tutorialPrefab, tutorialSpawnPoint);
        var tutorialUI = tutorialInstance.GetComponent<TutorialPrefabUI>();
        if (tutorialUI != null)
        {
            tutorialUI.Initialize(introTutorialData);
        }
    }
}
