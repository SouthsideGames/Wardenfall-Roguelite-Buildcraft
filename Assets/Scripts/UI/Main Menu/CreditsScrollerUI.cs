using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class CreditsScrollerUI : MonoBehaviour
{
    [Header("ELEMENTS:")]
    [SerializeField] private float scrollSpeed;
    [SerializeField] private float endYPosition = 1000f;
    private RectTransform rt;

    [SerializeField] private bool isIntroScroller = false;
    private bool hasFinished = false;

    private void Awake() => rt = GetComponent<RectTransform>();

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

        if(isIntroScroller)
        {
            if (rt.anchoredPosition.y >= endYPosition)
            {
                hasFinished = true;
                UIManager.Instance.CompleteIntro();
            }
        }

    }
}
