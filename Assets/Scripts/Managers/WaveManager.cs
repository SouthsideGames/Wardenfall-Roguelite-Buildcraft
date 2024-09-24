using System;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class WaveManager : MonoBehaviour
{
    [Header("ELEMENTS:")]
    [SerializeField] private CharacterManager character;
    
    [Header("SETTINGS:")]
    [SerializeField] private float waveDuration;
    private float timer;
    private bool hasWaveStarted;
    private int currentWaveIndex;

    [Header("WAVES SETTINGS:")]
    [SerializeField] private Wave[] wave;
    private List<float> localCounters = new List<float>();

    void Start()
    {
        
        StartWave(currentWaveIndex);
    }

    // Update is called once per frame
    void Update()
    {
        if(!hasWaveStarted)
           return;

        if(timer < waveDuration)
            ManageCurrentWave();
        else
            ManageWaveTransition();
    }
    
    private void ManageCurrentWave()
    {
        Wave currentWave = wave[currentWaveIndex];

        for(int i = 0; i < currentWave.segments.Count; i++)
        {   
            WaveSegment segment = currentWave.segments[i];  

            float tStart = segment.spawnPercentage.x / 100 * waveDuration;
            float tEnd = segment.spawnPercentage.y / 100 * waveDuration;    

            if(timer < tStart || timer > tEnd)
               continue;

            float timeSinceSegmentStart = timer - tStart;
            float spawnDelay = 1f / segment.spawnFrequency;

            if(timeSinceSegmentStart / spawnDelay > localCounters[i])
            {
                Instantiate(segment.prefab, GetSpawnPosition(), Quaternion.identity, transform);
                localCounters[i]++;
            }
        }

        timer += Time.deltaTime;    

    }

    private void ManageWaveTransition()
    {
        currentWaveIndex++;
        StartWave(currentWaveIndex);
    }

    private Vector2 GetSpawnPosition()
    {
        Vector2 direction = UnityEngine.Random.onUnitSphere;
        Vector2 offset = direction.normalized * UnityEngine.Random.Range(6, 10);
        Vector2 targetPosition = (Vector2)character.transform.position + offset;

        targetPosition.x = Mathf.Clamp(targetPosition.x, -18, 18);
        targetPosition.y = Mathf.Clamp(targetPosition.y, -8, 8);
        
        return targetPosition;
    }

    private void StartWave(int _waveIndex)
    {
        Debug.Log("Starting Wave " + _waveIndex);

        localCounters.Clear();

        foreach(WaveSegment segment in wave[_waveIndex ].segments)
            localCounters.Add(1);

        timer = 0;
        hasWaveStarted = true;
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