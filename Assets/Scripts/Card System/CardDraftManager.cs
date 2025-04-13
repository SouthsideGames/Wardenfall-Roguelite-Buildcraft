using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CardDraftManager : MonoBehaviour
{
    [SerializeField] private List<CardSO> allCards; // Assigned in inspector or loaded at runtime
    [SerializeField] private CharacterManager characterManager;

    [Header("Draft Settings")]
    [SerializeField] private int cardsPerDraft = 3;

    public void TriggerCardDraft()
    {
        List<CardSO> unlockedCards = allCards
            .Where(card => card.isUnlocked)
            .OrderBy(x => Random.value)
            .Take(cardsPerDraft)
            .ToList();

        CardDraftUI.Instance.ShowCardDraft(unlockedCards, OnCardSelected);
    }

    private void OnCardSelected(CardSO selectedCard)
    {
        characterManager.CharacterCards.AddCard(selectedCard);
    }

    public void SetCardPool(List<CardSO> newPool)
    {
        allCards = newPool;
    }
}