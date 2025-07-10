using System.Collections.Generic;
using UnityEngine;

public class CardInGameUIManager : MonoBehaviour
{
    [SerializeField] private Transform cardSlotContainer;
    [SerializeField] private InGameCardSlotUI cardSlotPrefab;
    [SerializeField] private CharacterManager characterManager;

    private bool enduranceBuffActive = false;

    private const string EnduranceCardID = "S-011";
    private const float EnduranceAbsorbPercent = .1f;

    private void OnEnable()
    {
        CharacterCards.OnDeckChanged += RefreshCardDisplay;
        RefreshCardDisplay();
    }

    private void OnDisable()
    {
        CharacterCards.OnDeckChanged -= RefreshCardDisplay;
    }

    private void Update()
    {
        bool anyCooldowns = false;

        foreach (Transform child in cardSlotContainer)
        {
            if (child.TryGetComponent<InGameCardSlotUI>(out var cardSlot))
            {
                if (cardSlot.IsCoolingDown())
                {
                    anyCooldowns = true;
                }
            }
        }

        EnduranceBuff(anyCooldowns);
    }

    private void EnduranceBuff(bool anyCooldowns)
    {
        if (characterManager.cards.HasCard(EnduranceCardID))
        {
            if (anyCooldowns && !enduranceBuffActive)
            {
                enduranceBuffActive = true;
                characterManager.stats.BoostStat(Stat.CritChance, EnduranceAbsorbPercent);
            }
            else if (!anyCooldowns && enduranceBuffActive)
            {
                enduranceBuffActive = false;
                characterManager.stats.RevertBoost(Stat.CritChance);
            }
        }
        else if (enduranceBuffActive)
        {
            enduranceBuffActive = false;
            characterManager.stats.RevertBoost(Stat.CritChance);
        }
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

    public void ResetAllCooldowns()
    {
        foreach (Transform child in cardSlotContainer)
        {
            if (child.TryGetComponent<InGameCardSlotUI>(out var cardSlot))
            {
                cardSlot.ResetCooldown();
            }
        }
    }

      public void ReduceRandomCooldown(string excludeCardID)
    {
        List<InGameCardSlotUI> validSlots = new List<InGameCardSlotUI>();

        foreach (Transform child in cardSlotContainer)
        {
            if (child.TryGetComponent<InGameCardSlotUI>(out var cardSlot))
            {
                if (cardSlot.IsCoolingDown() && cardSlot.AssignedCard.cardID != excludeCardID)
                {
                    validSlots.Add(cardSlot);
                }
            }
        }

        if (validSlots.Count > 0)
        {
            var selected = validSlots[Random.Range(0, validSlots.Count)];
            selected.ReduceCooldownBy(2f);
            Debug.Log("Arcane Coolant triggered: Cooldown reduced for a random card.");
        }
    }
}
