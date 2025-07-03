using System.Collections.Generic;
using UnityEngine;

public class ProgressionUnlockDatabase : MonoBehaviour
{
    public List<ProgressionUnlockDataSO> allUnlocks;

    private Dictionary<string, ProgressionUnlockDataSO> unlockLookup;

    private void Awake()
    {
        unlockLookup = new Dictionary<string, ProgressionUnlockDataSO>();
        foreach (var unlock in allUnlocks)
        {
            if (!string.IsNullOrEmpty(unlock.unlockID))
                unlockLookup[unlock.unlockID] = unlock;
        }
    }

    public ProgressionUnlockDataSO GetUnlockByID(string id)
    {
        unlockLookup.TryGetValue(id, out var unlock);
        return unlock;
    }

    public List<ProgressionUnlockDataSO> GetAllUnlocks() => allUnlocks;
}