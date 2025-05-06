using System.Collections.Generic;
using UnityEngine;

public class UnlockDatabase : MonoBehaviour
{
    public static UnlockDatabase Instance;
    public List<UnlockDataSO> allUnlocks;

    private Dictionary<string, UnlockDataSO> unlockLookup;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            unlockLookup = new Dictionary<string, UnlockDataSO>();
            foreach (var unlock in allUnlocks)
            {
                if (!string.IsNullOrEmpty(unlock.unlockID))
                    unlockLookup[unlock.unlockID] = unlock;
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public UnlockDataSO GetUnlockByID(string id)
    {
        unlockLookup.TryGetValue(id, out var unlock);
        return unlock;
    }
}