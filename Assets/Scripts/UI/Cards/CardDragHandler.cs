using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("ELEMENTS:")]
    [SerializeField] private Canvas canvas; // Reference to the Canvas for proper UI dragging
    [SerializeField] private RectTransform cardRectTransform; // Reference to the card's RectTransform
    [SerializeField] private CanvasGroup canvasGroup; // To control visibility during drag
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

        // Bring the card to the front
        cardRectTransform.SetParent(canvas.transform, true);

        canvasGroup.alpha = 0.6f; // Make the card semi-transparent
        canvasGroup.blocksRaycasts = false; // Allow raycasts to pass through during drag
    }

    public void OnDrag(PointerEventData eventData)
    {
        cardRectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
       Debug.Log($"Pointer entered: {eventData.pointerEnter?.name}");

        canvasGroup.alpha = 1f; // Restore visibility
        canvasGroup.blocksRaycasts = true; // Restore raycast blocking

        if (eventData.pointerEnter != null && eventData.pointerEnter.CompareTag("ActiveDeck"))
        {
            Debug.Log("Dropped in ActiveDeck");
            if (deckManager.TryAddCardToActiveDeck(cardData))
            {
                Destroy(gameObject);
            }
            else
            {
                Debug.Log("Not enough space in ActiveDeck");
                ResetPosition();
            }
        }
        else if (eventData.pointerEnter != null && eventData.pointerEnter.CompareTag("DeckList"))
        {
            Debug.Log("Dropped in DeckList");
            deckManager.RemoveCardFromActiveDeck(cardData);
        }
        else
        {
            Debug.Log("Dropped in an invalid area");
            ResetPosition();
        }

        cardRectTransform.SetParent(originalParent, true);
    }

    public CardSO GetCardData()
    {
        return cardData;
    }

    public void ResetPosition()
    {
        cardRectTransform.anchoredPosition = originalPosition;
        cardRectTransform.SetParent(originalParent, true);
    }
}
