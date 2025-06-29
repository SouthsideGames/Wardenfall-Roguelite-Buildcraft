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
    [SerializeField] private float minDistanceBetweenSpawns = 2f;
    [SerializeField] private int maxEnemiesOnScreen = 50;

    [Header("EVO SETTINGS:")]
    [SerializeField] private GameObject evoCrystalPrefab;
    [SerializeField] [Range(0f, 1f)] private float baseCrystalSpawnChance = 0.05f;
    [SerializeField] [Range(0f, 1f)] private float performanceBonusMultiplier = 0.10f;

    private List<float> localCounters = new List<float>();
    private float currentViewerScore = 0.5f;
    private Wave currentWave;
    private float playerPerformanceScore = 1f;
    private int enemiesKilledThisWave = 0;
    private float damageTakenThisWave = 0f;
    private float timeSinceLastKill = 0f;
    private bool droppedBelow20Triggered = false;
    private List<Vector2> recentSpawnPoints = new List<Vector2>();
    private float lastTickSecond = -1f;
    private bool hasSpawnedAnyEnemy = false;

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
        {
            HandleWaveProgression();
            AdjustViewerScore(-0.01f * Time.deltaTime);
            ViewerScoreAdjustments();

            if (hasSpawnedAnyEnemy && transform.childCount == 0)
            {
                Debug.Log("[WaveManager] Ending wave early—no enemies remaining.");
                WaveWrapUp();
            }
        }
        else
            WaveWrapUp();
    }


   private void StartWave(int waveIndex)
    {
        InitializeWave(waveIndex);
        AudioManager.Instance.PlayCrowdAmbience();
        hasWaveStarted = true;
        timer = 0;
        timeSinceLastKill = 0f;
        hasSpawnedAnyEnemy = false;

        FindAnyObjectByType<CardInGameUIManager>()?.ResetAllCooldowns();

        UpdateUIForWaveStart();

        ApplyHyperModeEffects();

        if (ChallengeManager.IsActive(ChallengeMode.RogueRoulette))
        {
            if (ChallengeManager.Instance.TryGetLastRouletteEffect(out Stat stat, out float mult))
            {
                string change = mult > 1f ? "Boosted" : "Reduced";
                string display = $"Rogue Roulette: {stat} {change} (x{mult})";
                UIManager.Instance?.ShowToast(display);
            }
        }
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

                hasSpawnedAnyEnemy = true;

                AdjustEnemyDifficulty(enemy.GetComponent<Enemy>());

                localCounters[index]++;
                
                AdjustViewerScore(+0.01f); 

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

    #region Viewer Score Management
    public void AdjustViewerScore(float amount)
    {
        currentViewerScore = Mathf.Clamp01(currentViewerScore + amount);
        UIManager.Instance?.viewerRatingSlider.UpdateBar(currentViewerScore);
    }

    public void ReportKill()
    {
        enemiesKilledThisWave++;
        AdjustViewerScore(+0.05f);
        timeSinceLastKill = 0f;
        
        TrySpawnEvoCrystal();
    }

    public void ReportPlayerHit()
    {
        damageTakenThisWave += 10f;
        AdjustViewerScore(-0.05f);
    }

    private void ViewerScoreAdjustments()
    {
        if (character.controller.MoveDirection.sqrMagnitude > 0.01f)
            AdjustViewerScore(0.01f * Time.deltaTime);

        timeSinceLastKill += Time.deltaTime;

        if (timeSinceLastKill >= 5f)
        {
            AdjustViewerScore(-0.10f);
            timeSinceLastKill = 0f;
        }

        float currentHealthRatio = character.health.CurrentHealth / character.stats.GetStatValue(Stat.MaxHealth);
        if (!droppedBelow20Triggered && currentHealthRatio < 0.2f)
        {
            AdjustViewerScore(-0.10f);
            droppedBelow20Triggered = true;
        }
    }

    #endregion


   private void WaveWrapUp()
    {
        OnWaveCompleted?.Invoke();

        float currentHealth = CharacterManager.Instance.health.CurrentHealth;
        float maxHealth = character.stats.GetStatValue(Stat.MaxHealth);

        if (maxHealth > 0 && (currentHealth / maxHealth) < 0.10f)
            AudioManager.Instance?.PlayCrowdReaction(CrowdReactionType.Gasp);

        int baseXP = 100;
        float bonusMultiplier = Mathf.Lerp(0.5f, 2f, currentViewerScore);
        int xpEarned = Mathf.RoundToInt(baseXP * bonusMultiplier);
        ProgressionManager.Instance.AddXP(xpEarned);

        AudioManager.Instance?.StopAmbientLoop();

        MissionIncrement();
        DefeatAllEnemies();
        hasWaveStarted = false;

        // Check if run is over
        if (currentWaveIndex + 1 >= wave.Length)
        {
            EndGame();
            return;
        }

        // 🌀 Rogue Roulette: Apply random stat effect after the wave ends
        if (ChallengeManager.IsActive(ChallengeMode.RogueRoulette))
        {
            CharacterStats stats = character.stats;
            Array statTypes = Enum.GetValues(typeof(Stat));
            Stat randomStat = (Stat)statTypes.GetValue(UnityEngine.Random.Range(0, statTypes.Length));
            float multiplier = UnityEngine.Random.value > 0.5f ? 1.5f : 0.5f;

            stats.ApplyTemporaryModifier(randomStat, multiplier, 30f);

            ChallengeManager.Instance.SetLastRouletteEffect(randomStat, multiplier);
        }

        currentWaveIndex++;
        Time.timeScale = 1f;
        GameManager.Instance.SetGameState(GameState.Progression);
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
        float remaining = Mathf.Max(0f, waveDuration - timer);
        TimeSpan timeSpan = TimeSpan.FromSeconds(remaining);
        string formatted = string.Format("{0:D2}:{1:D2}", timeSpan.Minutes, timeSpan.Seconds);

        // Set color based on time
        if (remaining <= 5f)
        {
            ui.SetTimerColor(Color.red);
            ui.FlashTimerUI();
        }
        else if (remaining <= 15f)
            ui.SetTimerColor(Color.yellow);
        else
            ui.SetTimerColor(Color.white);

        if (remaining <= 10f)
        {
            int currentSecond = Mathf.FloorToInt(remaining);
            if (currentSecond != Mathf.FloorToInt(lastTickSecond))
            {
                AudioManager.Instance?.PlaySFX(ui.tickSound);
                lastTickSecond = currentSecond;
            }
        }

        ui.UpdateTimerText(formatted);
    }

    private void EndGame()
    {
        var stats = StatisticsManager.Instance.currentStatistics;
        stats.CurrentRunDuration = Time.time - GameManager.Instance.runStartTime;
        stats.MostEffectiveWeaponInRun = CharacterManager.Instance.weapon.GetMostEffectiveWeapon();
        
        ui.StageCompleted();
        UIManager.Instance.UpdateStageCompletionPanel();
        GameManager.Instance.SetGameState(GameState.StageCompleted);
    }


    private Vector2 GetSpawnPosition()
    {
        Vector2 playerPos = character.transform.position;

        float angle = UnityEngine.Random.Range(0f, Mathf.PI * 2f);
        float radius = UnityEngine.Random.Range(12f, 16f); // Distance off-screen

        return new Vector2(
            playerPos.x + Mathf.Cos(angle) * radius,
            playerPos.y + Mathf.Sin(angle) * radius
        );
    }


    private void TrySpawnEvoCrystal()
    {
        float performanceBoost = Mathf.Clamp01(playerPerformanceScore * performanceBonusMultiplier);
        float spawnChance = baseCrystalSpawnChance + performanceBoost;

        if (UnityEngine.Random.value < spawnChance)
        {
            Vector2 spawnPos = GetSpawnPosition();
            Instantiate(evoCrystalPrefab, spawnPos, Quaternion.identity);
        }
    }


    private Vector2 GetFormationSpawnPosition(int enemyIndex)
    {
        Vector2 basePos = (Vector2)character.transform.position;
        float radius = UnityEngine.Random.Range(6f, 10f);
        float spacing = 1.5f;

        switch (currentWave.segments.Count % 11)
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
                        new Vector2(diagOffset, -diagOffset));
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
                Vector2 dir = arm switch
                {
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

    private void UpdateUIForWaveStart() => ui.UpdateWaveText($"CAM: {(currentWaveIndex + 1).ToString("D2")}");

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
    

    private void ApplyHyperModeEffects()
    {
        string[] hyperModeIDs = { "HyperModeT3", "HyperModeT2", "HyperModeT1" };

        foreach (string id in hyperModeIDs)
        {
            if (TraitManager.Instance.HasTrait(id, out int stack))
            {
                TraitTier tier = TraitManager.Instance.GetTraitTier(id, stack);
                if (tier != null)
                {
                    switch (id)
                    {
                        case "HyperModeT1":
                            waveDuration *= 0.85f; // 15% reduction
                            Time.timeScale = 1.5f;
                            break;
                        case "HyperModeT2":
                            waveDuration *= 0.75f; // 25% reduction
                            Time.timeScale = 2.0f;
                            break;
                        case "HyperModeT3":
                            waveDuration *= 0.65f; // 35% reduction
                            Time.timeScale = 2.5f;
                            break;
                    }

                    Debug.Log($"[HyperMode] Applied {id} → Wave Duration: {waveDuration:F2}s | TimeScale: {Time.timeScale}");
                    return;
                }
            }
        }

        // If no trait is found, ensure normal time scale
        Time.timeScale = 1f;
    }


    private void CharacterDeathCallback() => GameManager.Instance.SetGameState(GameState.GameOver);
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
