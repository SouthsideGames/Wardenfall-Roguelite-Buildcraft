using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using SouthsideGames.SaveManager;

public class LoreManager : MonoBehaviour
{
    public static LoreManager Instance;

    [SerializeField] private List<LoreShardSO> allShardAssets;

    private HashSet<int> unlockedShardIds = new();
    private HashSet<int> readShardIds = new();

    private const string SaveKey = "lore_progress";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        LoadProgress();
    }

    public List<LoreShardSO> GetUnlockedShards()
    {
        return allShardAssets.Where(s => unlockedShardIds.Contains(s.id)).ToList();
    }

    public void UnlockShard(int id)
    {
        if (unlockedShardIds.Add(id))
            SaveProgress();
    }

    public void MarkAsRead(int id)
    {
        if (readShardIds.Add(id))
            SaveProgress();
    }

    public bool IsShardRead(int id)
    {
        return readShardIds.Contains(id);
    }

    public void ResetLoreProgress()
    {
        unlockedShardIds.Clear();
        readShardIds.Clear();
        SaveProgress();
    }

    private void SaveProgress()
    {
        LoreSaveData data = new()
        {
            unlockedShardIds = unlockedShardIds.ToList(),
            readShardIds = readShardIds.ToList()
        };

        SaveManager.Save(this, SaveKey, data);
    }

    private void LoadProgress()
    {
        if (SaveManager.TryLoad(this, SaveKey, out object dataObj) && dataObj is LoreSaveData data)
        {
            unlockedShardIds = new HashSet<int>(data.unlockedShardIds);
            readShardIds = new HashSet<int>(data.readShardIds);
        }
    }
}

[System.Serializable]
public class LoreSaveData
{
    public List<int> unlockedShardIds = new();
    public List<int> readShardIds = new();
}
