using UnityEngine;
using SouthsideGames.SaveManager;
using System.Collections.Generic;
using NaughtyAttributes;
using System;

public class ProgressionManager : MonoBehaviour
{
    
    public static Action OnUnlockPointsChanged;
    
    public event Action<int> OnXPGained;
    public event Action<int> OnLevelUp;
    
    public static ProgressionManager Instance;
    public InGameProgressionUI inGameProgressionUI { get; private set; }
    public ProgressionUI progressionUI { get; private set; }
    public int ProgressionXP { get; private set; }
    public int UnlockPoints { get; private set; }
    public int LastGainedXP { get; private set; }
    public int PlayerLevel { get; private set; } = 1;

    private const string XP_KEY = "meta_xp";
    private const string POINTS_KEY = "unlock_points";
    private const string LEVEL_KEY = "player_level";
    private const string UNLOCKED_KEY = "unlocks";

    private HashSet<string> unlockedIDs = new();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Load();
        }
        else
            Destroy(gameObject);

        inGameProgressionUI = GetComponent<InGameProgressionUI>();
        progressionUI = GetComponent<ProgressionUI>();
    }

    void Start()
    {
        UpdateUI();
    }

    
    [Button]
    private void AddSigils()
    {
        UnlockPoints += 1;
        OnUnlockPointsChanged?.Invoke();
        UpdateUI();
        Save();
    } 

    public void AddMetaXP(int amount)
    {
        if (amount < 0) return;
        
        ProgressionXP += amount;
        LastGainedXP = amount;

        UnlockPoints += Mathf.FloorToInt(amount / 100f);
        OnUnlockPointsChanged?.Invoke();

        int startLevel = PlayerLevel;
        while (ProgressionXP >= GetXPForNextLevel())
        {
            ProgressionXP -= GetXPForNextLevel();
            PlayerLevel = Mathf.Min(PlayerLevel + 1, 99);
        }
        
        if (PlayerLevel != startLevel)
        {
            OnLevelUp?.Invoke(PlayerLevel);
        }

        OnXPGained?.Invoke(amount);
        Save();
    }

    public bool TryUnlock(string unlockID)
    {
        if (UnlockPoints <= 0 || unlockedIDs.Contains(unlockID))
            return false;
    
        var data = UnlockDatabase.Instance.GetUnlockByID(unlockID);
        if (data == null || data.cost > UnlockPoints)
            return false;
    
        unlockedIDs.Add(unlockID);
        UnlockPoints -= data.cost;
        OnUnlockPointsChanged?.Invoke();
    
        // Apply unlock effects
        switch (data.category)
        {
            case UnlockCategory.Card:
                UnlockCard(unlockID);
                break;
            case UnlockCategory.StatBooster:
                // Booster slots are checked dynamically by ID
                break;
            case UnlockCategory.ShopEconomy:
                // Shop effects are handled by MetaEffectManager
                break;
        }

        UpdateUI();
    
        Save();
        return true;
    }

    public bool IsUnlockActive(string unlockID) => unlockedIDs.Contains(unlockID);

    public void Unlock(string id)
    {
        if (!unlockedIDs.Contains(id))
        {
            unlockedIDs.Add(id);
            UnlockPoints--;
            Save(); // persist state
        }
    }

    private void UnlockCard(string unlockID)
    {
        CardSO card = CardLibrary.Instance.allCards.Find(c => c.cardID == unlockID);
        if (card != null)
            card.isUnlocked = true;
    }

    public int GetXPForNextLevel()
    {
        return Mathf.FloorToInt(1000 * Mathf.Pow(1.15f, PlayerLevel));
    }

    public void ClearLastGainedXP() => LastGainedXP = 0;

    private void UpdateUI()
    {
        SigilsUI[] sigilsUIs = FindObjectsByType<SigilsUI>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (SigilsUI sigilsUI in sigilsUIs)
            sigilsUI.UpdateText(UnlockPoints.ToString());
    }

    private void Save()
    {
        SaveManager.GameData.Add(XP_KEY, typeof(int), ProgressionXP.ToString());
        SaveManager.GameData.Add(POINTS_KEY, typeof(int), UnlockPoints.ToString());
        SaveManager.GameData.Add(LEVEL_KEY, typeof(int), PlayerLevel.ToString());
        SaveManager.GameData.Add(UNLOCKED_KEY, typeof(string), string.Join(",", unlockedIDs));
    }

    private void Load()
    {
        if (SaveManager.GameData.TryGetValue(XP_KEY, out var _, out var xpStr))
            ProgressionXP = int.Parse(xpStr);

        if (SaveManager.GameData.TryGetValue(POINTS_KEY, out var _, out var pointsStr))
            UnlockPoints = int.Parse(pointsStr);

        if (SaveManager.GameData.TryGetValue(LEVEL_KEY, out var _, out var levelStr))
            PlayerLevel = Mathf.Max(1, int.Parse(levelStr));

        if (SaveManager.GameData.TryGetValue(UNLOCKED_KEY, out var _, out var unlockedStr))
        {
            var ids = unlockedStr.Split(',');
            foreach (var id in ids)
                if (!string.IsNullOrEmpty(id))
                    unlockedIDs.Add(id);
        }
    }
}
