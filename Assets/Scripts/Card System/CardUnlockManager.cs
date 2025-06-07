using UnityEngine;
using SouthsideGames.SaveManager;

public class CardUnlockManager : MonoBehaviour
{
    public static CardUnlockManager Instance;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public bool CanUnlock(CardSO card)
    {
        return card != null &&
               !card.unlockData.unlocked &&
               CurrencyManager.Instance.HasEnoughCardCurrency(card.unlockData.unlockCost);
    }

    public bool TryUnlockCard(CardSO card)
    {
        if (!CanUnlock(card))
        {
            Debug.Log("Cannot unlock card. Either it's null, already unlocked, or not enough unlock tickets.");
            return false;
        }

        CurrencyManager.Instance.UseCardCurrency(card.unlockData.unlockCost);
        card.unlockData.unlocked = true;

        SaveManager.Save(this, card.cardID + "_Unlocked", true);
        Debug.Log($"{card.cardName} unlocked using {card.unlockData.unlockCost} unlock tickets.");

        AudioManager.Instance?.PlayCrowdReaction(CrowdReactionType.Whistle);
        return true;
    }

    public void LoadCardUnlockState(CardSO card)
    {
        if (card == null) return;

        if (SaveManager.TryLoad(this, card.cardID + "_Unlocked", out object unlocked))
            card.unlockData.unlocked = (bool)unlocked;
    }

    public void LoadAllCardUnlockStates(CardLibrary library)
    {
        if (library == null) return;

        foreach (var card in library.allCards)
            LoadCardUnlockState(card);
    }
}
