using System.Collections.Generic;
using UnityEngine;

public class UnlockRegistry : MonoBehaviour
{
    public List<UnlockDataSO> allUnlocks;

    public UnlockDataSO GetUnlock(string id) =>
        allUnlocks.Find(u => u.unlockID == id);
}

