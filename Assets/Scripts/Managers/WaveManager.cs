using System;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using SouthsideGames.DailyMissions;


[RequireComponent(typeof(WaveUI))]
public class WaveManager : MonoBehaviour, IGameStateListener
{
    public static Action OnWaveCompleted;

    [Header("ELEMENTS:")]
    [SerializeField] private CharacterManager character;
    private WaveUI ui;

    [Header("DIFFICULTY SETTINGS:")]
    [SerializeField] private float difficultyMultiplier = 1.0f;
    [SerializeField] private float difficultyIncrement = 0.1f;
    
    [Header("SETTINGS:")]
    [SerializeField] private float waveDuration;
    private float timer;
    private bool hasWaveStarted;
    private int currentWaveIndex;

    [Header("WAVES SETTINGS:")]
    [SerializeField] private Wave[] wave;
    private List<float> localCounters = new List<float>();

    [Header("GAME MODE:")]
    public GameMode selectedGameMode { get; private set; }

    private Wave currentWave;
    private float survivalScalingTimer;
    private int waveCompletionCount;  

    private void Awake()
    {
        ui = GetComponent<WaveUI>();

        CharacterHealth.OnCharacterDeath += CharacterDeathCallback;

    }

    private void OnDestroy()
    {
        CharacterHealth.OnCharacterDeath -= CharacterDeathCallback;
    }

    private void Update()
    {
        if (!hasWaveStarted) return;

        switch (selectedGameMode)
        {
            case GameMode.Survival:
                HandleSurvivalMode();
                break;

            default:
                if (timer < waveDuration)
                {
                    HandleWaveProgression();
                }
                else
                {
                    HandleWaveTransition();
                }
                break;
        }
    }

    private void StartWave(int waveIndex)
    {
        if (selectedGameMode == GameMode.Survival && hasWaveStarted) return; // Skip for Survival mode

        InitializeWave(waveIndex);
        hasWaveStarted = true;
        timer = 0;

        UpdateUIForWaveStart();
        Debug.Log($"Wave {waveIndex + 1} started in {selectedGameMode} mode.");
    }

    private void InitializeWave(int waveIndex)
    {
        localCounters.Clear();
        currentWave = wave[waveIndex];

        for (int i = 0; i < currentWave.segments.Count; i++)
        {
            var segment = currentWave.segments[i]; // Copy the segment

            segment.tStart = segment.spawnPercentage.x / 100 * waveDuration;
            segment.tEnd = segment.spawnPercentage.y / 100 * waveDuration;

            if (selectedGameMode == GameMode.Endless || selectedGameMode == GameMode.Survival)
                segment.spawnFrequency *= difficultyMultiplier;

            currentWave.segments[i] = segment; // Write the modified segment back to the list
            localCounters.Add(0);
        }
    }

    private void HandleWaveProgression()
    {
        SpawnWaveSegments();
        UpdateWaveTimer();
    }

    private void SpawnWaveSegments()
    {
        foreach (var (segment, index) in currentWave.segments.WithIndex())
        {
            if (timer < segment.tStart || timer > segment.tEnd) continue;

            float timeSinceStart = timer - segment.tStart;
            float spawnInterval = 1f / segment.spawnFrequency;

            if (timeSinceStart / spawnInterval > localCounters[index])
            {
                Instantiate(segment.prefab, GetSpawnPosition(), Quaternion.identity, transform);
                localCounters[index]++;

                if (segment.spawnOnce) localCounters[index] += Mathf.Infinity; // Stop further spawns
            }
        }
    }

    private void HandleWaveTransition()
    {
        OnWaveCompleted?.Invoke();
        MissionManager.Increment(MissionType.wavesCompleted, 1);
        DefeatAllEnemies();
        hasWaveStarted = false;
        currentWaveIndex++;

        switch (selectedGameMode)
        {
            case GameMode.WaveBased:
                HandleWaveBasedTransition();
                break;

            case GameMode.Endless:
            case GameMode.BossRush:
            case GameMode.ObjectiveBased:
                HandleEndlessOrBossRushTransition();
                break;
        }
    }
 
    private void HandleWaveBasedTransition()
    {
        if (currentWaveIndex >= wave.Length)
        {
            EndStage();
        }
        else
        {
            ApplyDifficultyScaling();
            GameManager.Instance.WaveCompletedCallback();
        }
    }

    private void HandleEndlessOrBossRushTransition()
    {
        waveCompletionCount++;

        if (waveCompletionCount % 5 == 0)
            ApplyDifficultyScaling();

        if (selectedGameMode == GameMode.Endless)
            currentWaveIndex %= wave.Length; // Loop waves indefinitely

        if (currentWaveIndex >= wave.Length && selectedGameMode != GameMode.Endless)
            EndStage();

        StartWave(currentWaveIndex);
    }

    private void HandleSurvivalMode()
    {
        SpawnWaveSegments();
        timer += Time.deltaTime;
        survivalScalingTimer += Time.deltaTime;

        if (survivalScalingTimer >= 120f) // 2 minutes
        {
            ApplyDifficultyScaling();
            survivalScalingTimer = 0;
        }
    }

    private void UpdateWaveTimer()
    {
        timer += Time.deltaTime;
        ui.UpdateTimerText(((int)(waveDuration - timer)).ToString());
    }

    private void EndStage()
    {
        ui.StageCompleted();
        GameManager.Instance.SetGameState(GameState.StageCompleted);
    }

    private void ApplyDifficultyScaling()
    {
        difficultyMultiplier += difficultyIncrement;
        Debug.Log($"Difficulty scaled. Current multiplier: {difficultyMultiplier}");
    }

    private Vector2 GetSpawnPosition()
    {
        Vector2 direction = UnityEngine.Random.insideUnitCircle.normalized;
        Vector2 offset = direction * UnityEngine.Random.Range(6, 10);
        Vector2 target = (Vector2)character.transform.position + offset;

        if(!CameraManager.Instance.UseInfiniteMap)
        {
            target.x = Mathf.Clamp(target.x, -Constants.arenaSize.x / 2, Constants.arenaSize.x / 2);
            target.y = Mathf.Clamp(target.y, -Constants.arenaSize.y / 2, Constants.arenaSize.y / 2);
        }

        return target;  
    }

    private void UpdateUIForWaveStart()
    {
        if (selectedGameMode == GameMode.BossRush)
            ui.UpdateWaveText($"Boss Wave {currentWaveIndex + 1}");
        else
            ui.UpdateWaveText($"Wave {currentWaveIndex + 1} / {wave.Length}");
    }

    private void DefeatAllEnemies()
    {
        foreach (Enemy enemy in transform.GetComponentsInChildren<Enemy>())
            enemy.DieAfterWave();
    }

    public void GameStateChangedCallback(GameState gameState)
    {
        switch (gameState)
        {
            case GameState.Game:
                StartWave(currentWaveIndex);
                break;
            case GameState.GameOver:
                hasWaveStarted = false;
                DefeatAllEnemies();
                break;
        }
    }

    public void SetGameMode(GameMode mode)
    {
        selectedGameMode = mode;
        Debug.Log($"Game mode set to: {selectedGameMode}");
    }

    private void CharacterDeathCallback()
    {
        character.health.OnCharacterDeathMission(selectedGameMode);
    }
}

public static class Extensions
{
    public static IEnumerable<(T item, int index)> WithIndex<T>(this IEnumerable<T> source)
    {
        int i = 0;
        foreach (var item in source)
            yield return (item, i++);
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
    [Tooltip("Spawn this prefab once?")]
    public bool spawnOnce;

    [HideInInspector] public float tStart;
    [HideInInspector] public float tEnd;
}