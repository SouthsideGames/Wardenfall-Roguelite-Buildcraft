using System;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using SouthsideGames.DailyMissions;
using UnityEngine.Pool;


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

    [Header("SURVIVAL MODE SETTINGS:")]
    [SerializeField] private float spawnFrequency = 2.0f;
    [SerializeField] private List<SurvivalWaveGroup> survivalGroups;
    [SerializeField] private List<GameObject> bossPrefabs;
    [SerializeField] private float bossSpawnInterval = 180f; // 3 minutes
    private bool bossSpawned;
    public float survivalTimer { get; private set; }
    private float survivalScalingTimer;
    private float nextBossSpawnTime;
    public float SurvivalTime => survivalTimer;

    [Header("GAME MODE:")]
    public GameMode selectedGameMode { get; private set; }


    private Wave currentWave;
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

        if (selectedGameMode == GameMode.Survival)
        {
            survivalTimer = 0;
            survivalScalingTimer = 0;
            nextBossSpawnTime = bossSpawnInterval;
        }

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
        OnWaveCompleted?.Invoke();

        MissionIncrement();
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
         survivalTimer += Time.deltaTime;
        survivalScalingTimer += Time.deltaTime;

        if (survivalTimer >= nextBossSpawnTime)
        {
            SpawnSurvivalBoss();
            nextBossSpawnTime += Mathf.Max(60f, bossSpawnInterval * 0.9f);
        }

        if (survivalScalingTimer >= 120f)
        {
            ApplySurvivalDifficultyScaling();
            survivalScalingTimer = 0;
        }

        SpawnSurvivalEnemies();

        ui.UpdateTimerText($"{(int)survivalTimer / 60:D2}:{(int)survivalTimer % 60:D2}");
    }

    private void SpawnSurvivalEnemies()
    {
        var group = GetCurrentSurvivalGroup();
        if (group != null && Time.time >= timer)
        {
            var enemyPrefab = group.enemies[UnityEngine.Random.Range(0, group.enemies.Count)];
            Instantiate(enemyPrefab, GetSpawnPosition(), Quaternion.identity, transform);
            timer = Time.time + 1f / spawnFrequency;
        }
    }

    private SurvivalWaveGroup GetCurrentSurvivalGroup()
    {
        foreach (var group in survivalGroups)
        {
            if (survivalTimer >= group.startTime && survivalTimer < group.endTime)
            {
                return group;
            }
        }
        return null;
    }

    private void SpawnSurvivalBoss()
    {
        if (bossPrefabs.Count == 0) return;

        GameObject boss = bossPrefabs[UnityEngine.Random.Range(0, bossPrefabs.Count)];
        Instantiate(boss, GetSpawnPosition(), Quaternion.identity, transform);
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
        OnSurvivalCompleted?.Invoke();

        survivalTimer = 0;
        survivalScalingTimer = 0;
        nextBossSpawnTime = bossSpawnInterval;
    }

    private void ApplySurvivalDifficultyScaling()
    {
        difficultyMultiplier += difficultyIncrement;

        spawnFrequency = Mathf.Clamp(spawnFrequency + 0.1f, 0.5f, 10f);
        Debug.Log($"Difficulty scaled. Multiplier: {difficultyMultiplier}, Spawn Frequency: {spawnFrequency}");
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
            case GameState.SurvivalStageCompleted:
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
        {
            EndSurvivalStage(); // Trigger survival completion
        }
        else
        {
            GameManager.Instance.SetGameState(GameState.GameOver);
        }
    }

    private void MissionIncrement()
    {
        MissionManager.Increment(MissionType.complete50Waves, 1);
        MissionManager.Increment(MissionType.complete100Waves, 1);
        MissionManager.Increment(MissionType.complete200Waves, 1);
        MissionManager.Increment(MissionType.complete300Waves, 1);
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

[Serializable]
public class SurvivalWaveGroup
{
    public string name;
    public float startTime;
    public float endTime;
    public List<GameObject> enemies;
}