using System;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using SouthsideGames.DailyMissions;


[RequireComponent(typeof(WaveUI))]
public class WaveManager : MonoBehaviour, IGameStateListener
{
    public static Action OnWaveCompleted;           
    public static Action OnSurvivalCompleted;     

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

    [Header("WAVES BASED SETTINGS:")]
    [SerializeField] private Wave[] wave;

    [Header("BOSS RUSH SETTINGS:")]
    [SerializeField] private Wave[] bossWaves; 
    private List<float> localCounters = new List<float>();

    [Header("GAME MODE:")]
    public GameMode selectedGameMode { get; private set; }

    private Wave currentWave;
    public float survivalTimer {get; private set; } 
    private float survivalScalingTimer;
    private int waveCompletionCount;

    public float SurvivalTime => survivalTimer; // Expose survival time for XP calculation

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
                    HandleWaveProgression();
                else
                    HandleWaveTransition();
                break;
        }
    }

    private void StartWave(int waveIndex)
    {
        if (selectedGameMode == GameMode.Survival && hasWaveStarted) return;

        InitializeWave(waveIndex);
        hasWaveStarted = true;
        timer = 0;

        UpdateUIForWaveStart();
    }

    private void InitializeWave(int waveIndex)
    {
        localCounters.Clear();
        currentWave = (selectedGameMode == GameMode.BossRush) ? bossWaves[waveIndex] : wave[waveIndex];

        for (int i = 0; i < currentWave.segments.Count; i++)
        {
            var segment = currentWave.segments[i];

            segment.tStart = segment.spawnPercentage.x / 100 * waveDuration;
            segment.tEnd = segment.spawnPercentage.y / 100 * waveDuration;

            if (selectedGameMode == GameMode.Survival)
                segment.spawnFrequency *= difficultyMultiplier;

            currentWave.segments[i] = segment;
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

                if (segment.spawnOnce) localCounters[index] += Mathf.Infinity;
            }
        }
    }

    private void HandleWaveTransition()
    {
        // Trigger XP gain for wave-based and boss rush modes
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

            case GameMode.BossRush:
                HandleBossRushTransition();
                break;
        }
    }

    private void HandleWaveBasedTransition()
    {
        if (currentWaveIndex >= wave.Length)
            EndWaveBasedStage();
        else
            GameManager.Instance.WaveCompletedCallback();
    }

    private void HandleBossRushTransition()
    {
        waveCompletionCount++;

        if (currentWaveIndex >= bossWaves.Length)
            EndWaveBasedStage();
        else
            StartWave(currentWaveIndex);
    }

    private void HandleSurvivalMode()
    {
        SpawnWaveSegments();
        timer += Time.deltaTime;
        survivalTimer += Time.deltaTime; // Track survival time
        survivalScalingTimer += Time.deltaTime;

        if (survivalScalingTimer >= 120f)
        {
            ApplySurvivalDifficultyScaling();
            survivalScalingTimer = 0;
        }
    }

    private void UpdateWaveTimer()
    {
        timer += Time.deltaTime;
        ui.UpdateTimerText(((int)(waveDuration - timer)).ToString());
    }

    private void EndWaveBasedStage()
    {
        ui.StageCompleted();
        GameManager.Instance.SetGameState(GameState.StageCompleted);
    }

    private void EndSurvivalStage()
    {
        ui.StageCompleted();
        GameManager.Instance.SetGameState(GameState.SurvivalStageCompleted);

        // Trigger XP gain based on survival duration
        OnSurvivalCompleted?.Invoke();
    }

    private void ApplySurvivalDifficultyScaling()
    {
        difficultyMultiplier += difficultyIncrement;
        Debug.Log($"Difficulty scaled. Current multiplier: {difficultyMultiplier}");
    }

    private Vector2 GetSpawnPosition()
    {
        Vector2 direction = UnityEngine.Random.insideUnitCircle.normalized;
        Vector2 offset = direction * UnityEngine.Random.Range(6, 10);
        return (Vector2)character.transform.position + offset;
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

        if (selectedGameMode == GameMode.Survival)
            EndSurvivalStage();
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