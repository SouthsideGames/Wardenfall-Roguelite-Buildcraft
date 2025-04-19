using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using NaughtyAttributes;
using SouthsideGames.DailyMissions;
using UnityEngine.Pool;


[RequireComponent(typeof(WaveUI))]
public class WaveManager : MonoBehaviour, IGameStateListener
{
    public static WaveManager Instance;
    public static Action OnWaveCompleted;

    [Header("ELEMENTS:")]
    [SerializeField] private CharacterManager character;
    private WaveUI ui;

    [Header("SETTINGS:")]
    [SerializeField] private float waveDuration;
    private float timer;
    private bool hasWaveStarted;
    private int currentWaveIndex;

    [Header("WAVES BASED SETTINGS:")]
    [SerializeField] private Wave[] wave;
    private List<float> localCounters = new List<float>();

    private Wave currentWave;

    [SerializeField] private float minDistanceBetweenSpawns = 2f;
    [SerializeField] private int maxEnemiesOnScreen = 50;
    private List<Vector2> recentSpawnPoints = new List<Vector2>();

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        ui = GetComponent<WaveUI>();
        CharacterHealth.OnCharacterDeath += CharacterDeathCallback;
    }

    private void OnDestroy() => CharacterHealth.OnCharacterDeath -= CharacterDeathCallback;


    private void Update()
    {
        if (!hasWaveStarted) return;

        if (timer < waveDuration)
            HandleWaveProgression();
        else
            HandleWaveTransition();

    }

    private void StartWave(int waveIndex)
    {
        InitializeWave(waveIndex);
        hasWaveStarted = true;
        timer = 0;

        UpdateUIForWaveStart();
    }

    private void InitializeWave(int waveIndex)
    {
        localCounters.Clear();
        recentSpawnPoints.Clear(); // Clear spawn points at the start of each wave
        currentWave = wave[waveIndex];

        for (int i = 0; i < currentWave.segments.Count; i++)
        {
            var segment = currentWave.segments[i];

            segment.tStart = segment.spawnPercentage.x / 100 * waveDuration;
            segment.tEnd = segment.spawnPercentage.y / 100 * waveDuration;

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
        // Check if we've hit enemy limit
        if (transform.childCount >= maxEnemiesOnScreen) return;

        foreach (var (segment, index) in currentWave.segments.WithIndex())
        {
            if (timer < segment.tStart || timer > segment.tEnd) continue;

            float timeSinceStart = timer - segment.tStart;
            float spawnInterval = 1f / segment.spawnFrequency;

            if (timeSinceStart / spawnInterval > localCounters[index])
            {
                Vector2 spawnPos = GetOptimizedSpawnPosition();

                // Spawn with dynamic difficulty adjustment
                GameObject enemy = Instantiate(segment.prefab, spawnPos, Quaternion.identity, transform);

                // Adjust enemy stats based on wave progression
                AdjustEnemyDifficulty(enemy.GetComponent<Enemy>());

                localCounters[index]++;

                if (segment.spawnOnce) 
                {
                    localCounters[index] += Mathf.Infinity;
                    // Special handling for boss segments
                    PrepareArenaForBoss();
                }

                // Clean up old spawn points
                CleanupSpawnHistory();
            }
        }
    }

    private Vector2 GetOptimizedSpawnPosition()
    {
        Vector2 spawnPos;
        int attempts = 0;
        const int maxAttempts = 10;

        do
        {
            spawnPos = GetSpawnPosition();
            attempts++;
        } 
        while (IsTooCloseToOtherSpawns(spawnPos) && attempts < maxAttempts);

        recentSpawnPoints.Add(spawnPos);
        return spawnPos;
    }

    private bool IsTooCloseToOtherSpawns(Vector2 position)
    {
        return recentSpawnPoints.Any(p => Vector2.Distance(p, position) < minDistanceBetweenSpawns);
    }

    private void CleanupSpawnHistory()
    {
        if (recentSpawnPoints.Count > 10)
        {
            recentSpawnPoints.RemoveAt(0);
        }
    }

    private void AdjustEnemyDifficulty(Enemy enemy)
    {
        if (enemy == null) return;

        float difficultyMultiplier = 1f + (currentWaveIndex * 0.1f);
        enemy.maxHealth = Mathf.RoundToInt(enemy.maxHealth * difficultyMultiplier);
        enemy.contactDamage = Mathf.RoundToInt(enemy.contactDamage * difficultyMultiplier);
    }

    private void PrepareArenaForBoss()
    {
        // Clear regular enemies when boss spawns
        StartCoroutine(GradualEnemyClear());
    }

    private IEnumerator GradualEnemyClear()
    {
        var enemies = GetComponentsInChildren<Enemy>()
            .Where(e => !(e is Boss))
            .ToList();

        foreach (var enemy in enemies)
        {
            if (enemy != null)
            {
                enemy.DieAfterWave();
                yield return new WaitForSeconds(0.1f);
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

       HandleWaveBasedTransition();
    }

    private void HandleWaveBasedTransition()
    {
        if (currentWaveIndex >= wave.Length)
            EndWaveBasedStage();
        else
            GameManager.Instance.WaveCompletedCallback();
    }

    public bool IsCurrentWaveBoss() 
    {
        foreach (var segment in currentWave.segments) 
        {
            if (segment.spawnOnce)
                return true;
        }
            return false;
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


    private Vector2 GetSpawnPosition()
    {
        Vector2 direction = UnityEngine.Random.insideUnitCircle.normalized;
        Vector2 offset = direction * UnityEngine.Random.Range(6, 10);
        return (Vector2)character.transform.position + offset;
    }

    private void UpdateUIForWaveStart() => ui.UpdateWaveText($"Trial {currentWaveIndex + 1} / {wave.Length}");


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


    private void CharacterDeathCallback()
    {
        character.health.OnCharacterDeathMission();
        GameManager.Instance.SetGameState(GameState.GameOver);
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