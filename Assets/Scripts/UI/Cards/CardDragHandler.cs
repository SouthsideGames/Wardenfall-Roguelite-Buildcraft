using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    
    [Header("ELEMENTS:")]
    [SerializeField] private Canvas canvas;
    [SerializeField] private RectTransform cardRectTransform;
    [SerializeField] private CanvasGroup canvasGroup;

    private CardSO cardData;
    private DeckManager deckManager;

    private Vector2 originalPosition;
    private Transform originalParent;

    public void Configure(CardSO cardSO, DeckManager manager)
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

    public void OnBeginDrag(PointerEventData eventData)
    {
        originalPosition = cardRectTransform.anchoredPosition;
        originalParent = cardRectTransform.parent;

        cardRectTransform.SetParent(canvas.transform, true);

        canvasGroup.alpha = 0.6f;
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData) => cardRectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;

    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log($"Pointer entered: {eventData.pointerEnter?.name}");

        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;

        if (eventData.pointerEnter != null && eventData.pointerEnter.CompareTag("ActiveDeck"))
        {
            Debug.Log("Dropped in ActiveDeck");
            if (deckManager.TryAddCardToActiveDeck(cardData, gameObject))
            {
                // Card UI is now removed in TryAddCardToActiveDeck
            }
            else
            {
                Debug.Log("Not enough space in ActiveDeck");
                ResetPosition();
            }
        }
        else if (eventData.pointerEnter != null && eventData.pointerEnter.CompareTag("DeckList"))
        {
            ResetPosition(); // Reset back to DeckList if dropped incorrectly
        }
        else
        {
            Debug.Log("Dropped in an invalid area");
            ResetPosition();
        }

        cardRectTransform.SetParent(originalParent, true);
    }

    public CardSO GetCardData() => cardData;

    public void ResetPosition()
    {
        cardRectTransform.anchoredPosition = originalPosition;
        cardRectTransform.SetParent(originalParent, true);
    }
}
