using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DropZoneUI : MonoBehaviour, IDropHandler
{
     [Header("Settings")]
    [SerializeField] private string zoneType; // "ActiveDeck" or "DeckList"
    [SerializeField] private DeckManager deckManager;

    public void OnDrop(PointerEventData eventData)
    {
        MiniDecklistCardUI miniIcon = eventData.pointerDrag.GetComponent<MiniDecklistCardUI>();
        CardDragHandlerUI cardDragHandler = eventData.pointerDrag.GetComponent<CardDragHandlerUI>();

        if (miniIcon != null)
        {
            HandleMiniCardDrop(miniIcon);
        }
        else if (cardDragHandler != null)
        {
            HandleCardDrop(cardDragHandler);
        }
    }

    private void HandleCardDrop(CardDragHandlerUI draggedCard)
    {
        CardSO cardData = draggedCard.GetCardData();

        if (zoneType == "ActiveDeck")
        {
            if (deckManager.TryAddCardToActiveDeck(cardData, draggedCard.gameObject))
            {
                Debug.Log($"Card {cardData.CardName} added to ActiveDeck.");
            }
            else
            {
                Debug.Log($"Not enough space for card {cardData.CardName} in ActiveDeck.");
                draggedCard.ResetPosition();
            }
        }
        else if (zoneType == "DeckList")
        {
            draggedCard.ResetPosition();
        }
    }

    private void HandleMiniCardDrop(MiniDecklistCardUI miniCard)
    {
        CardSO cardData = miniCard.GetCardData();

        if (zoneType == "DeckList")
        {
            deckManager.ReturnMiniCardToDeck(cardData, miniCard);
        }
        else
        {
            miniCard.ResetPosition();
        }
    }
}
