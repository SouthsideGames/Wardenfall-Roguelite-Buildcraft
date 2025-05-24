using System.Collections.Generic;
using UnityEngine;

public class ProgressionBoosterRegistry : MonoBehaviour
{
    public static ProgressionBoosterRegistry Instance;
    public List<StatBoosterSO> allBoosters;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }
}