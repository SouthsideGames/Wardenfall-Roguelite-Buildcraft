
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using NaughtyAttributes;
using SouthsideGames.DailyMissions;

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
    public int currentWaveIndex { get; private set; }

    [Header("WAVES BASED SETTINGS:")]
    [SerializeField] private Wave[] wave;
    private List<float> localCounters = new List<float>();

    private Wave currentWave;
    private float playerPerformanceScore = 1f;
    private int enemiesKilledThisWave = 0;
    private float damageTakenThisWave = 0f;

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

        FindAnyObjectByType<InGameCardUIManager>()?.ResetAllCooldowns();

        UpdateUIForWaveStart();
    }

    private void InitializeWave(int waveIndex)
    {
        localCounters.Clear();
        recentSpawnPoints.Clear(); 
        currentWave = wave[waveIndex];

        for (int i = 0; i < currentWave.segments.Count; i++)
        {
            var segment = currentWave.segments[i];

            segment.tStart = segment.spawnPercentage.x / 100 * waveDuration;
            segment.tEnd = segment.spawnPercentage.y / 100 * waveDuration;
            
            // Randomly select a prefab from the possible options
            if (segment.enemiesToSpawn != null && segment.enemiesToSpawn.Length > 0)
            {
                segment.selectedPrefab = segment.enemiesToSpawn[UnityEngine.Random.Range(0, segment.enemiesToSpawn.Length)];
            }

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

        UpdatePlayerPerformance();

        foreach (var (segment, index) in currentWave.segments.WithIndex())
        {
            if (timer < segment.tStart || timer > segment.tEnd) continue;

            float timeSinceStart = timer - segment.tStart;
            float spawnInterval = 1f / segment.spawnFrequency;

            if (timeSinceStart / spawnInterval > localCounters[index])
            {
                Vector2 spawnPos = GetOptimizedSpawnPosition();

                GameObject enemy = Instantiate(segment.selectedPrefab, spawnPos, Quaternion.identity, transform);

                AdjustEnemyDifficulty(enemy.GetComponent<Enemy>());

                localCounters[index]++;

                if (segment.bossWave) 
                {
                    localCounters[index] += Mathf.Infinity;
                    PrepareArenaForBoss();
                }

                CleanupSpawnHistory();
            }
        }
    }

    private Vector2 GetOptimizedSpawnPosition()
    {
        Vector2 spawnPos;
        int attempts = 0;
        const int maxAttempts = 10;

        bool useFormation = false;
        float formationThreshold = 0.8f;
        float formationChance = (playerPerformanceScore - formationThreshold) / (1f - formationThreshold);

        if (playerPerformanceScore > formationThreshold)
            useFormation = UnityEngine.Random.value < formationChance;

        do
        {
            
            if (useFormation)
                spawnPos = GetFormationSpawnPosition(transform.childCount);
            else
                spawnPos = GetSpawnPosition();

            attempts++;
        }
        while (IsTooCloseToOtherSpawns(spawnPos) && attempts < maxAttempts);

        recentSpawnPoints.Add(spawnPos);
        return spawnPos;
    }

    private bool IsTooCloseToOtherSpawns(Vector2 position) => recentSpawnPoints.Any(p => Vector2.Distance(p, position) < minDistanceBetweenSpawns);

    private void CleanupSpawnHistory()
    {
        if (recentSpawnPoints.Count > 10)
            recentSpawnPoints.RemoveAt(0);
    }

    private void UpdatePlayerPerformance()
    {
        float killScore = enemiesKilledThisWave / (maxEnemiesOnScreen * 0.5f);
        float damageScore = Mathf.Max(0.5f, 1f - (damageTakenThisWave / 100f));
        playerPerformanceScore = (killScore + damageScore) / 2f;

        enemiesKilledThisWave = 0;
        damageTakenThisWave = 0f;
    }

    private void AdjustEnemyDifficulty(Enemy enemy)
    {
        if (enemy == null) return;

        float baseMultiplier = 1f + (currentWaveIndex * 0.1f);
        float performanceMultiplier = Mathf.Lerp(0.8f, 1.2f, playerPerformanceScore);
        float finalMultiplier = baseMultiplier * performanceMultiplier;

        enemy.maxHealth = Mathf.RoundToInt(enemy.maxHealth * finalMultiplier);
        enemy.contactDamage = Mathf.RoundToInt(enemy.contactDamage * finalMultiplier);
    }

    private void PrepareArenaForBoss() => StartCoroutine(GradualEnemyClear());

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
         Debug.Log($"[Wave End] Timer hit: {timer:F2}");
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
        {
            EndWaveBasedStage();
        }
        else
        {
           
            if (IsCurrentWaveBoss())
            {
                Debug.Log("[WaveManager] Showing Major Draft after boss wave.");
                FindAnyObjectByType<CardDraftManager>()?.ShowMajorDraft();
            }

            GameManager.Instance.WaveCompletedCallback();
        }
    }

    public bool IsCurrentWaveBoss() 
    {
        foreach (var segment in currentWave.segments) 
        {
            if (segment.bossWave)
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
        // Update final stage statistics
        var stats = StatisticsManager.Instance.currentStatistics;
        stats.CurrentRunDuration = Time.time - GameManager.Instance.runStartTime;
        stats.MostEffectiveWeaponInRun = CharacterManager.Instance.weapon.GetMostEffectiveWeapon();
        
        ui.StageCompleted();
        UIManager.Instance.UpdateStageCompletionPanel();
        GameManager.Instance.SetGameState(GameState.StageCompleted);
    }


    private Vector2 GetSpawnPosition()
    {
        Vector2 characterPos = (Vector2)character.transform.position;
        Vector2 direction = UnityEngine.Random.insideUnitCircle.normalized;
        Vector2 offset = direction * UnityEngine.Random.Range(6, 10);
        Vector2 spawnPos = characterPos + offset;

        float padding = 1f; 
        float minX = -Constants.arenaSize.x/2 + padding;
        float maxX = Constants.arenaSize.x/2 - padding;
        float minY = -Constants.arenaSize.y/2 + padding;
        float maxY = Constants.arenaSize.y/2 - padding;

        spawnPos.x = Mathf.Clamp(spawnPos.x, minX, maxX);
        spawnPos.y = Mathf.Clamp(spawnPos.y, minY, maxY);

        return spawnPos;
    }


    private Vector2 GetFormationSpawnPosition(int enemyIndex)
    {
        Vector2 basePos = (Vector2)character.transform.position;
        float radius = UnityEngine.Random.Range(6f, 10f);
        float spacing = 1.5f;

        switch(currentWave.segments.Count % 11)
        {
            case 0: //Circle Formation
                float angle = enemyIndex * 360f / maxEnemiesOnScreen * Mathf.Deg2Rad;
                return basePos + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius;
            case 1: // V formation
                float forwardOffset = enemyIndex * spacing;
                float sideOffset = enemyIndex * spacing * 0.5f;
                return basePos + new Vector2(sideOffset, forwardOffset);
            case 2: // Square formation
                int row = enemyIndex / 4;
                int col = enemyIndex % 4;
                return basePos + new Vector2((col - 1.5f) * spacing, (row - 1.5f) * spacing) * 2;
            case 3: //X formation
                float diagOffset = enemyIndex * spacing * 0.7f;
                return basePos + (enemyIndex % 2 == 0 ? 
                        new Vector2(diagOffset, diagOffset) : 
                        new Vector2(diagOffset, - diagOffset));
            case 4: //Double circle formation
                float innerRadius = radius * 0.5f;
                float angleDouble = enemyIndex * 360f / (maxEnemiesOnScreen / 2) * Mathf.Deg2Rad;
                return basePos + new Vector2(Mathf.Cos(angleDouble), Mathf.Sin(angleDouble)) * (enemyIndex % 2 == 0 ? radius : innerRadius);
            case 5: //Diamond formation
                int layer = Mathf.Min(enemyIndex / 4, 3);
                float diamondAngle = enemyIndex % 4 * 90f * Mathf.Deg2Rad;
                float layerRadius = (layer + 1) * spacing;
                return basePos + new Vector2(Mathf.Cos(diamondAngle), Mathf.Sin(diamondAngle)) * layerRadius;
            case 6: // Pentagon formation
                float pentagonAngle = enemyIndex * 72f * Mathf.Deg2Rad;
                return basePos + new Vector2(Mathf.Cos(pentagonAngle), Mathf.Sin(pentagonAngle)) * radius;
            case 7: //Hexagon Formation
                float hexAngle = enemyIndex * 60f * Mathf.Deg2Rad;
                return basePos + new Vector2(Mathf.Cos(hexAngle), Mathf.Sin(hexAngle)) * radius;
            case 8: //Cross formation
                int arm = enemyIndex % 4;
                int dist = enemyIndex / 4;
                Vector2 dir = arm switch {
                    0 => Vector2.up,
                    1 => Vector2.right,
                    2 => Vector2.down,
                    _ => Vector2.left
                };
                return basePos + dir * (dist + 1) * spacing;
            case 9: // Sprial Formation
                float spiralAngle = enemyIndex * 30f * Mathf.Deg2Rad;
                float spiralRadius = enemyIndex * 0.05f * spacing;
                return basePos + new Vector2(Mathf.Cos(spiralAngle), Mathf.Sin(spiralAngle)) * spiralRadius;
            
            default: // Random formation with minimum spacing
                return GetOptimizedSpawnPosition();
        }
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
    [Tooltip("Possible enemies that can be spawned in this wave")]
    public GameObject[] enemiesToSpawn;
    [Tooltip("Spawn this prefab once?")]
    public bool bossWave;

    [HideInInspector] public float tStart;
    [HideInInspector] public float tEnd;
    [HideInInspector] public GameObject selectedPrefab;
}
