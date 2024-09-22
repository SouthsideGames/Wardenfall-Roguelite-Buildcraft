using System;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class WaveManager : MonoBehaviour
{
    
    [Header("SETTINGS:")]
    [SerializeField] private float waveDuration;

    [Header("WAVES SETTINGS:")]
    [SerializeField] private Wave[] wave;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

[Serializable]
public struct Wave
{
    public string name;
    public List<WaveSegment> segments;
}

[Serializable]
public struct WaveSegment
{
    [Tooltip("What percentage of time will objects be spawned?")]
    [MinMaxSlider(0, 100)] public Vector2 spawnPercentage;
    [Tooltip("How many to spawn per second?")]
    public float spawnFrequency;
    [Tooltip("What will be spawned in this wave?")]
    public GameObject prefab;
}