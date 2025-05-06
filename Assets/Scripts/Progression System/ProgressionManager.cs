using UnityEngine;
using SouthsideGames.SaveManager;
using System.Collections.Generic;

public class ProgressionManager : MonoBehaviour
{
    public static ProgressionManager Instance;
    public InGameProgressionUI ProgressionUI;

    public int MetaXP { get; private set; }
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
    }

    public void AddMetaXP(int amount)
    {
        MetaXP += amount;
        LastGainedXP = amount;

        UnlockPoints += Mathf.FloorToInt(amount / 100f);

        while (MetaXP >= GetXPForNextLevel())
        {
            MetaXP -= GetXPForNextLevel();
            PlayerLevel++;
        }

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

    public int GetXPForNextLevel()
    {
        return Mathf.FloorToInt(1000 * Mathf.Pow(1.15f, PlayerLevel));
    }

    public void ClearLastGainedXP() => LastGainedXP = 0;

    private void Save()
    {
        SaveManager.GameData.Add(XP_KEY, typeof(int), MetaXP.ToString());
        SaveManager.GameData.Add(POINTS_KEY, typeof(int), UnlockPoints.ToString());
        SaveManager.GameData.Add(LEVEL_KEY, typeof(int), PlayerLevel.ToString());
        SaveManager.GameData.Add(UNLOCKED_KEY, typeof(string), string.Join(",", unlockedIDs));
    }

    private void Load()
    {
        if (SaveManager.GameData.TryGetValue(XP_KEY, out var _, out var xpStr))
            MetaXP = int.Parse(xpStr);

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
