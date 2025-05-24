using System.Collections.Generic;
using UnityEngine;

public class ProgressionUnlockRegistry : MonoBehaviour
{
    public List<ProgressionUnlockDataSO> allUnlocks;

    public ProgressionUnlockDataSO GetUnlock(string id) =>
        allUnlocks.Find(u => u.unlockID == id);
}

