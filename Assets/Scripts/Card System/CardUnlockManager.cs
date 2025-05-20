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
        return card != null && !card.isUnlocked && CurrencyManager.Instance.HasEnoughCardPoints(card.requiredCardPoints);
    }

    public bool TryUnlockCard(CardSO card)
    {
        if (!CanUnlock(card))
        {
            Debug.Log("Cannot unlock card. Either it's null, already unlocked, or not enough card points.");
            return false;
        }

        CurrencyManager.Instance.UseCardPoints(card.requiredCardPoints);
        card.isUnlocked = true;

        SaveManager.Save(this, card.cardName + "_Unlocked", true);
        Debug.Log($"{card.cardName} unlocked using {card.requiredCardPoints} card points.");
        return true;
    }

    public void LoadCardUnlockState(CardSO card)
    {
        if (card == null) return;

        if (SaveManager.TryLoad(this, card.cardName + "_Unlocked", out object unlocked))
            card.isUnlocked = (bool)unlocked;
    }

    public void LoadAllCardUnlockStates(CardLibrary library)
    {
        if (library == null) return;

        foreach (var card in library.allCards)
            LoadCardUnlockState(card);
    }
}
