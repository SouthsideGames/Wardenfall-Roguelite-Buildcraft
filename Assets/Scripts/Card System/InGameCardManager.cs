using System.Collections.Generic;
using UnityEngine;

public class InGameCardUIManager : MonoBehaviour
{
    [SerializeField] private Transform cardSlotContainer;
    [SerializeField] private InGameCardSlotUI cardSlotPrefab;
    [SerializeField] private CharacterManager characterManager;

    private void OnEnable()
    {
        CharacterCards.OnDeckChanged += RefreshCardDisplay;
        RefreshCardDisplay();
    }

    private void OnDisable()
    {
        CharacterCards.OnDeckChanged -= RefreshCardDisplay;
    }

    public void RefreshCardDisplay()
    {
        foreach (Transform child in cardSlotContainer)
        {
            Destroy(child.gameObject);
        }

        var deck = characterManager.cards.Deck;
        foreach (var card in deck)
        {
            var slot = Instantiate(cardSlotPrefab, cardSlotContainer);
            slot.Setup(card);
        }
    }
}
