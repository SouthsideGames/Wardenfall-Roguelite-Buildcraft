using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System;

public class CardHandlerUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerDownHandler
{
     public static Action<CardSO> OnButtonPressed;

    [Header("ELEMENTS:")]
    [SerializeField] private Canvas canvas;
    [SerializeField] private RectTransform cardRectTransform;
    [SerializeField] private CanvasGroup canvasGroup;

    private CardSO cardData;
    private CardManager deckManager;

    private Vector2 originalPosition;
    private Transform originalParent;

    private float pointerDownTime;
    private const float ClickThreshold = 0.2f; // Adjust based on your needs
    private bool isDragging;

    public void Configure(CardSO cardSO, CardManager manager)
    {
        cardData = cardSO;
        deckManager = manager;
    }

    private void Awake()
    {
        if (!canvas) canvas = GetComponentInParent<Canvas>();
        if (!canvasGroup) canvasGroup = GetComponent<CanvasGroup>();
        if (!cardRectTransform) cardRectTransform = GetComponent<RectTransform>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        pointerDownTime = Time.time;
        isDragging = false;
        Debug.Log("Pointer down on card");
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        isDragging = true;

        originalPosition = cardRectTransform.anchoredPosition;
        originalParent = cardRectTransform.parent;

        cardRectTransform.SetParent(canvas.transform, true);

        canvasGroup.alpha = 0.6f;
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isDragging) return;
        cardRectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!isDragging) return;

        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;

        if (eventData.pointerEnter != null && eventData.pointerEnter.CompareTag("ActiveDeck"))
        {
            if (deckManager.TryAddCardToActiveDeck(cardData, gameObject))
            {
                Debug.Log("Dropped in ActiveDeck");
            }
            else
            {
                Debug.Log("Not enough space in ActiveDeck");
                ResetPosition();
            }
        }
        else
        {
            Debug.Log("Dropped in an invalid area");
            ResetPosition();
        }

        cardRectTransform.SetParent(originalParent, true);
    }


    public void OnPointerUp(PointerEventData eventData)
    {
        Debug.Log("PointerUp called");
        if (!isDragging && Time.time - pointerDownTime <= ClickThreshold)
        {
            Debug.Log($"Card clicked: {cardData?.CardName}");
            OnButtonPressed?.Invoke(cardData);
        }
    }

    public CardSO GetCardData() => cardData;

    public void ResetPosition()
    {
        cardRectTransform.anchoredPosition = originalPosition;
        cardRectTransform.SetParent(originalParent, true);
    }
}
