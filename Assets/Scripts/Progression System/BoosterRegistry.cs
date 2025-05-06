using System.Collections.Generic;
using UnityEngine;

public class BoosterRegistry : MonoBehaviour
{
    public static BoosterRegistry Instance;
    public List<StatBoosterSO> allBoosters;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }
}