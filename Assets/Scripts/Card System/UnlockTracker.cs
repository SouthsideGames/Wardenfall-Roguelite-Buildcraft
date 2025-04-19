using System.Collections.Generic;
using UnityEngine;
using SouthsideGames.SaveManager;

public class UnlockTracker : MonoBehaviour, IWantToBeSaved
{
    private HashSet<string> unlockedCardIDs = new HashSet<string>();
    [SerializeField] private CardLibrary cardLibrary;

    public void Initialize(List<CardSO> allCards)
    {
        foreach (CardSO card in allCards)
        {
            card.isUnlocked = unlockedCardIDs.Contains(card.cardID);
        }
    }

    public void UnlockCard(CardSO card)
    {
        if (!unlockedCardIDs.Contains(card.cardID))
        {
            unlockedCardIDs.Add(card.cardID);
            card.isUnlocked = true;
            Save();
        }
    }

    public bool IsCardUnlocked(CardSO card) => unlockedCardIDs.Contains(card.cardID);

    public float GetUnlockPercentage(List<CardSO> allCards)
    {
        if (allCards.Count == 0) return 0f;
        int unlockedCount = allCards.FindAll(c => c.isUnlocked).Count;
        return (float)unlockedCount / allCards.Count * 100f;
    }

    public void Save()
    {
        SaveManager.Save(this, "UnlockedCards", new List<string>(unlockedCardIDs));
    }

    public void Load()
    {
        if (SaveManager.TryLoad(this, "UnlockedCards", out object value))
        {
            List<string> savedList = value as List<string>;
            unlockedCardIDs = new HashSet<string>(savedList);
        }
    }

    [ContextMenu("Unlock All Cards (Dev Debug)")]
    public void UnlockAll()
    {
        foreach (var card in cardLibrary.allCards)
        {
            unlockedCardIDs.Add(card.cardID);
            card.isUnlocked = true;
        }
        Save();
        Debug.Log("[UnlockTracker] All cards unlocked for debug/testing.");
    }
} 
