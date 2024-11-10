using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class CreditsScrollerUI : MonoBehaviour
{
    [Header("ELEMENTS:")]
    [SerializeField] private float scrollSpeed;
    private RectTransform rt;

    private void Awake() 
    {
        rt = GetComponent<RectTransform>();
    }

    private void OnEnable() 
    {
        rt.anchoredPosition = rt.anchoredPosition.With(y: 0);   
    }

    private void Update() 
    {
        rt.anchoredPosition += Vector2.up * Time.deltaTime * scrollSpeed;    
    }
}
