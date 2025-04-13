using System;
using System.Collections.Generic;
using UnityEngine;

public class CardDraftUI : MonoBehaviour
{
    public static CardDraftUI Instance;

    private Action<CardSO> onCardChosen;
    private CardSO pendingReplacementCard;
    private List<CardSO> currentDeck;
    private int currentDeckCost;
    private int maxDeckCost;

    private void Awake()
    {
        if(Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void ShowCardDraft(List<CardSO> draftOptions, Action<CardSO> onSelectCallback)
    {
        onCardChosen = onSelectCallback;
        // TODO: Spawn UI card buttons and hook them up to OnCardSelected()
    }

    public void OnCardSelected(CardSO selectedCard)
    {
        onCardChosen?.Invoke(selectedCard);
        Debug.Log("[DraftUI] Card selected: \" + selectedCard.cardName");
    }

    public void ShowReplaceCardPrompt(CardSO newCard, List<CardSO> deck, int currentCost, int maxCost)
    {
        pendingReplacementCard = newCard;
        currentDeck = deck;
        currentDeckCost = currentCost;
        maxDeckCost = maxCost;

        // TODO: Spawn replacement UI showing deck cards with Replace or Skip option
    }

    public void OnReplaceSelected(int replaceIndex)
    {
        CharacterManager.Instance.cards.ReplaceCard(replaceIndex, pendingReplacementCard);
        pendingReplacementCard = null;
    }

    public void OnSkipSelected()
    {
        CharacterManager.Instance.cards.SkipCard();
        pendingReplacementCard = null;
    }

    public void HideReplaceCardPrompt()
    {
        // TODO: Hide replacement UI
        Debug.Log("[DraftUI] Hiding replace prompt");
    }
}