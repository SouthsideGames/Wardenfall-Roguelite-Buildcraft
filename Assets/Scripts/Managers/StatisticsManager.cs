using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using NaughtyAttributes;
using System;

public class StatisticsManager : MonoBehaviour
{
  public static StatisticsManager Instance { get; private set; }

    public GameStatistics currentStatistics = new GameStatistics();
    private string statsFilePath;

    private float lastPlayTimeCheck;
    private bool isTrackingPlayTime = false;
    public int CurrentRunKills {get; private set;}
    public int CurrentWaveCompleted {get; private set;}
    public int CurrentCandyCollected {get; private set;}
    public int CurrentChestCollected {get; private set;}
    public int CurrentLevelUp {get; private set;}

    private void Awake()
    {
        if(Instance == null)
            Instance = this;
        else
            Destroy(gameObject);


        Candy.OnCollected += TotalCandyCollectedHandler;
        Chest.OnCollected += TotalChestCollectedHandler;
        GameManager.OnWaveCompleted += TotalWaveCompletedHandler;
        Enemy.OnEnemyKilled += TotalEnemyKillsHandler;
        CharacterHealth.OnCharacterDeath += TotalCharacterDeathHandler;
        GameManager.OnGamePaused += OnGamePausedHandler;
        GameManager.OnGameResumed += OnGameResumedHandler;


    }

    private void OnDestroy() 
    {
        Candy.OnCollected -= TotalCandyCollectedHandler;
        Chest.OnCollected -= TotalChestCollectedHandler;
        GameManager.OnWaveCompleted -= TotalWaveCompletedHandler;
        Enemy.OnEnemyKilled -= TotalEnemyKillsHandler;
        CharacterHealth.OnCharacterDeath -= TotalCharacterDeathHandler;
        GameManager.OnGamePaused -= OnGamePausedHandler;
        GameManager.OnGameResumed -= OnGameResumedHandler;
    }

    private void Start()
    {
        statsFilePath = Application.persistentDataPath + "/gamestats.json";
        LoadStats();

        lastPlayTimeCheck = Time.time;
    }

    private void Update()
    {
        if (isTrackingPlayTime)
        {
            // Accumulate playtime by calculating the difference since the last check
            float deltaTime = Time.time - lastPlayTimeCheck;
            currentStatistics.TotalPlayTime += deltaTime;  // Increment total playtime

            // Update the last checked time to the current time
            lastPlayTimeCheck = Time.time;
        }
    }


    public void SaveStats()
    {
        string json = JsonUtility.ToJson(currentStatistics, true);
        System.IO.File.WriteAllText(statsFilePath, json);
    }

    public void LoadStats()
    {
        if (System.IO.File.Exists(statsFilePath))
        {
            string json = System.IO.File.ReadAllText(statsFilePath);
            currentStatistics = JsonUtility.FromJson<GameStatistics>(json);
        }
        else
        {
            currentStatistics = new GameStatistics();
        }
    }

    [Button]
    public void ClearSaveFile()
    {
        // Delete the save file if it exists
        if (System.IO.File.Exists(statsFilePath))
        {
            System.IO.File.Delete(statsFilePath);
        }

        // Reset the in-memory stats to default values
        currentStatistics = new GameStatistics();

        Debug.Log("All saved data has been cleared!");

        // Optionally, you could save the reset stats immediately to overwrite any existing data
        SaveStats();
    }

    public void StartNewRun()
    {
        CurrentRunKills = 0;
        CurrentLevelUp = 0;
        CurrentCandyCollected = 0;
        CurrentChestCollected = 0;
        CurrentWaveCompleted = 0;
    }

    public void EndRun()
    {
        
        if (CurrentRunKills > currentStatistics.MostKillsInARun)
            currentStatistics.MostKillsInARun = CurrentRunKills;

        if (CurrentLevelUp > currentStatistics.MostLevelUpsInARun)
            currentStatistics.MostLevelUpsInARun = CurrentLevelUp; 

        if (CurrentCandyCollected > currentStatistics.MostCandyCollectedInARun)
            currentStatistics.MostCandyCollectedInARun = CurrentCandyCollected;

        if (CurrentChestCollected > currentStatistics.MostChestsCollectedInARun)
            currentStatistics.MostChestsCollectedInARun = CurrentChestCollected;

        if (CurrentWaveCompleted > currentStatistics.MostWavesCompletedInARun)
            currentStatistics.MostWavesCompletedInARun  = CurrentChestCollected;

        // Save the updated stats
        SaveStats();
    }


    #region STATISTICS FUNCTIONS

    private void TotalCandyCollectedHandler(Candy _candy)
    {
        CurrentCandyCollected++;
        currentStatistics.TotalCandyCollected++;
        SaveStats();
    }

    private void TotalChestCollectedHandler(Chest _chest)
    {
        CurrentChestCollected++;
        currentStatistics.TotalChestCollected++;
        SaveStats();
    }

    private void TotalWaveCompletedHandler()
    {
        CurrentWaveCompleted++;
        currentStatistics.TotalWavesCompletedInARun++;
        SaveStats();
    }

    private void TotalEnemyKillsHandler()
    {
        CurrentRunKills++;
        currentStatistics.TotalKills++;
        SaveStats();
    }

    private void TotalCharacterDeathHandler()
    {
        // Increment totalDeaths
        currentStatistics.TotalDeaths++;
        SaveStats(); 
    }

    public void OnGameResumedHandler() => StartTimer();
    public void OnGamePausedHandler() =>  StopTimer();

    public void StartTimer()
    {
        isTrackingPlayTime = true;
        lastPlayTimeCheck = Time.time;
    }

    public void StopTimer()
    {
        if (isTrackingPlayTime)
        {
            // Stop tracking and accumulate playtime
            isTrackingPlayTime = false;

            float deltaTime = Time.time - lastPlayTimeCheck;
            currentStatistics.TotalPlayTime += deltaTime;

            SaveStats();
        }
    }

    public void TotalLevelUpsInARun() => CurrentLevelUp++;

    private void OnApplicationQuit()
    {
        StopTimer();
        EndRun();
    }

    #endregion

}

[Serializable]
public class GameStatistics
{
     // Highest values recorded in a single run
    public int MostWavesCompletedInARun;    // Most waves completed in one run
    public int MostKillsInARun;             // Most kills in a single run
    public int MostLevelUpsInARun;          // Most level-ups in a single run
    public int MostChestsCollectedInARun;   // Most chests collected in a single run
    public int MostCandyCollectedInARun;    // Most candy collected in a single run

    // Cumulative stats across all runs
    public int TotalWavesCompletedInARun;   // Total waves completed
    public int TotalKills;                  // Total kills across all runs
    public int TotalDeaths;                 // Total deaths across all runs
    public int TotalCandyCollected;         // Total candy collected across all runs
    public float TotalPlayTime;               // Total time played
    public int TotalChestCollected;          //Total chest collected across all runs

}
