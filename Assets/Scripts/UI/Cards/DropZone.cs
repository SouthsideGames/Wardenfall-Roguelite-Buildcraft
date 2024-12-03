using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DropZone : MonoBehaviour, IDropHandler
{
     [Header("Settings")]
    [SerializeField] private string zoneType; // "ActiveDeck" or "DeckList"
    [SerializeField] private DeckManager deckManager; // Reference to the DeckManager

    public void OnDrop(PointerEventData eventData)
    {
        // Get the dragged card
        CardDragHandler draggedCard = eventData.pointerDrag.GetComponent<CardDragHandler>();
        if (draggedCard != null)
        {
            HandleCardDrop(draggedCard);
        }
    }

    private void HandleCardDrop(CardDragHandler draggedCard)
    {
        CardSO cardData = draggedCard.GetCardData();

        if (zoneType == "ActiveDeck")
        {
          
            if (deckManager.TryAddCardToActiveDeck(cardData))
            {
                Destroy(draggedCard.gameObject);
            }
            else
            {
                Debug.Log("Not enough space in the Active Deck.");
                draggedCard.ResetPosition();
            }
        }
        else if (zoneType == "DeckList")
            deckManager.RemoveCardFromActiveDeck(cardData);
        else
        {
            Debug.LogWarning($"Unknown zone type: {zoneType}");
            draggedCard.ResetPosition(); 
        }
    }
}
