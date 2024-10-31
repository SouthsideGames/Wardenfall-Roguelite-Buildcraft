using System;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

[RequireComponent(typeof(WaveUI))]
public class WaveManager : MonoBehaviour, IGameStateListener
{
    [Header("ELEMENTS:")]
    [SerializeField] private CharacterManager character;
    private WaveUI ui;
    
    [Header("SETTINGS:")]
    [SerializeField] private float waveDuration;
    private float timer;
    private bool hasWaveStarted;
    private int currentWaveIndex;

    [Header("WAVES SETTINGS:")]
    [SerializeField] private Wave[] wave;
    private List<float> localCounters = new List<float>();

    
    private void Awake()
    {
        ui = GetComponent<WaveUI>(); 
    }

    // Update is called once per frame
    void Update()
    {
        if(!hasWaveStarted)
           return;

        if(timer < waveDuration)
        {
            ManageCurrentWave();

            string timerString = ((int)(waveDuration - timer)).ToString();

            ui.UpdateTimerText(timerString);
        }
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
        DefeatAllEnemies();
        hasWaveStarted = false;

        currentWaveIndex++;

        if (currentWaveIndex >= wave.Length)
        {
            ui.StageCompleted();

            GameManager.Instance.SetGameState(GameState.StageCompleted);
        }
        else
            GameManager.Instance.WaveCompletedCallback();
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

    public void GameStateChangedCallback(GameState _gameState)
    {
        switch(_gameState)
        {
            case GameState.Game:
                StartNextWave();
                break;
            case GameState.GameOver:
                hasWaveStarted = false;
                DefeatAllEnemies();
                break;
        }
    }

    private void StartWave(int _waveIndex)
    {
        StatisticsManager.Instance.StartTimer();
        StatisticsManager.Instance.StartNewRun();
        
        ui.UpdateWaveText("Wave " + (currentWaveIndex + 1) + " / " + wave.Length);

        localCounters.Clear();

        foreach(WaveSegment segment in wave[_waveIndex ].segments)
            localCounters.Add(1);

        timer = 0;
        hasWaveStarted = true;
    }

    private void StartNextWave()
    {
        StartWave(currentWaveIndex);
    }

    private void DefeatAllEnemies()
    {
        foreach(Enemy enemy in transform.GetComponentsInChildren<Enemy>())
            enemy.DieAfterWave();    
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