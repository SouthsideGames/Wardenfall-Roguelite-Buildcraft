using System.Collections.Generic;
using UnityEngine;
using System;
using NaughtyAttributes;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class StatisticsManager : MonoBehaviour
{
    public static StatisticsManager Instance { get; private set; }
    public GameStatistics currentStatistics = new GameStatistics();
    public int CurrentRunKills { get; private set; }
    public int CurrentWaveCompleted { get; private set; }
    public int CurrentMeatCollected { get; private set; }
    public int CurrentChestCollected { get; private set; }
    public int CurrentLevelUp { get; private set; }

    private string statsFilePath;
    private float lastPlayTimeCheck;
    private bool isTrackingPlayTime = false;

    private Dictionary<string, UsageInfo> characterUsageDict = new Dictionary<string, UsageInfo>();
    private Dictionary<string, UsageInfo> weaponUsageDict = new Dictionary<string, UsageInfo>();

    [Header("ELEMENTS:")]
    [SerializeField] private Button recordButton;
    [SerializeField] private GameObject recordContainer;
    [SerializeField] private GameObject collectionContainer;

    [Header("WEIGHTS:")]
    [SerializeField] private int killXPWeight = 2;
    [SerializeField] private int waveXPWeight = 25;
    [SerializeField] private int meatXPWeight = 1;
    [SerializeField] private float viewerScoreMinMultiplier = 0.5f;
    [SerializeField] private float viewerScoreMaxMultiplier = 2f;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);


        Meat.OnCollected += TotalMeatCollectedHandler;
        Chest.OnCollected += TotalChestCollectedHandler;
        GameManager.OnWaveCompleted += TotalWaveCompletedHandler;
        Enemy.OnEnemyKilled += TotalEnemyKillsHandler;
        CharacterHealth.OnCharacterDeath += TotalCharacterDeathHandler;
        GameManager.OnGamePaused += OnGamePausedHandler;
        GameManager.OnGameResumed += OnGameResumedHandler;

        collectionContainer.SetActive(false);
        recordContainer.SetActive(true);


    }

    private void OnDestroy()
    {
        Meat.OnCollected -= TotalMeatCollectedHandler;
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
        EventSystem.current.SetSelectedGameObject(recordButton.gameObject);
    }

    private void Update()
    {
        if (isTrackingPlayTime)
        {
            float deltaTime = Time.time - lastPlayTimeCheck;
            currentStatistics.TotalPlayTime += deltaTime;

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

    public void StartNewRun()
    {
        CurrentRunKills = 0;
        CurrentLevelUp = 0;
        CurrentMeatCollected = 0;
        CurrentChestCollected = 0;
        CurrentWaveCompleted = 0;
    }

    public void EndRun()
    {

        if (CurrentRunKills > currentStatistics.MostKillsInARun)
            currentStatistics.MostKillsInARun = CurrentRunKills;

        if (CurrentLevelUp > currentStatistics.MostLevelUpsInARun)
            currentStatistics.MostLevelUpsInARun = CurrentLevelUp;

        if (CurrentMeatCollected > currentStatistics.MostMeatCollectedInARun)
            currentStatistics.MostMeatCollectedInARun = CurrentMeatCollected;

        if (CurrentChestCollected > currentStatistics.MostChestsCollectedInARun)
            currentStatistics.MostChestsCollectedInARun = CurrentChestCollected;

        if (CurrentWaveCompleted > currentStatistics.MostWavesCompletedInARun)
            currentStatistics.MostWavesCompletedInARun = CurrentChestCollected;

        SaveStats();
    }

    public void SwitchToSummary()
    {
        collectionContainer.SetActive(false);
        recordContainer.SetActive(true);
       
    }

    public void SwitchToCollection()
    {
        collectionContainer.SetActive(true);
        recordContainer.SetActive(false);
    }

    private void OnApplicationQuit()
    {
        StopTimer();
        EndRun();
    }

    public void TurnOnRecordButton() => EventSystem.current.SetSelectedGameObject(recordButton.gameObject);


    #region STATISTICS FUNCTIONS

    private void TotalMeatCollectedHandler(Meat _meat)
    {
        CurrentMeatCollected++;
        currentStatistics.TotalMeatCollected++;
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
        currentStatistics.TotalWavesCompleted++;
        SaveStats();
    }

    public void TotalEnemyKillsHandler()
    {
        CurrentRunKills++;
        currentStatistics.TotalKills++;
        SaveStats();
    }

    private void TotalCharacterDeathHandler()
    {
        currentStatistics.TotalDeaths++;
        SaveStats();
    }

    public void OnGameResumedHandler() => StartTimer();
    public void OnGamePausedHandler() => StopTimer();

    public void StartTimer()
    {
        isTrackingPlayTime = true;
        lastPlayTimeCheck = Time.time;
    }

    public void StopTimer()
    {
        if (isTrackingPlayTime)
        {
            isTrackingPlayTime = false;

            float deltaTime = Time.time - lastPlayTimeCheck;
            currentStatistics.TotalPlayTime += deltaTime;

            SaveStats();
        }
    }

    public void TotalLevelUpsInARun() => CurrentLevelUp++;

    public void RecordCharacterUsage(string characterID)
    {
        if (!characterUsageDict.ContainsKey(characterID))
        {
            characterUsageDict[characterID] = new UsageInfo { UsageCount = 0, LastUsed = DateTime.Now };
        }

        characterUsageDict[characterID].UsageCount++;
        characterUsageDict[characterID].LastUsed = DateTime.Now;

        ConvertDictionariesToLists();
        SaveStats();
    }

    public void RecordWeaponUsage(string weaponID, int damageDealt)
    {
        if (!weaponUsageDict.ContainsKey(weaponID))
        {
            weaponUsageDict[weaponID] = new UsageInfo { UsageCount = 0, LastUsed = DateTime.Now };
        }

        weaponUsageDict[weaponID].UsageCount++;
        weaponUsageDict[weaponID].LastUsed = DateTime.Now;

        ConvertDictionariesToLists();
        SaveStats();
    }

    public void RecordCharacterWavesCompleted(string characterID)
    {
        if (!characterUsageDict.ContainsKey(characterID))
        {
            characterUsageDict[characterID] = new UsageInfo { UsageCount = 0, LastUsed = DateTime.Now };
        }

        if (characterUsageDict.TryGetValue(characterID, out UsageInfo usageInfo))
        {
            CharacterUsageEntry characterEntry = currentStatistics.CharacterUsageList.Find(entry => entry.CharacterID == characterID);

            if (characterEntry != null)
                characterEntry.WavesCompleted++;
        }

        ConvertDictionariesToLists();
        SaveStats();
    }

    public void RecordWeaponWavesCompleted(string weaponID)
    {
        if (!weaponUsageDict.ContainsKey(weaponID))
        {
            weaponUsageDict[weaponID] = new UsageInfo { UsageCount = 0, LastUsed = DateTime.Now };
        }

        if (weaponUsageDict.TryGetValue(weaponID, out UsageInfo usageInfo))
        {
            WeaponUsageEntry weaponEntry = currentStatistics.WeaponUsageList.Find(entry => entry.WeaponID == weaponID);

        }
        ConvertDictionariesToLists();
        SaveStats();
    }

    public void SetAverageViewerScoreForRun(float averageScore)
    {
        currentStatistics.AverageViewerScoreInRun = averageScore;

        if (averageScore > currentStatistics.HighestAverageViewerScore)
            currentStatistics.HighestAverageViewerScore = averageScore;

        SaveStats();
    }


    private void ConvertDictionariesToLists()
    {
        currentStatistics.CharacterUsageList.Clear();
        foreach (var entry in characterUsageDict)
        {
            CharacterUsageEntry characterEntry = new CharacterUsageEntry { CharacterID = entry.Key, UsageInfo = entry.Value };
            if (characterUsageDict.TryGetValue(entry.Key, out UsageInfo usageInfo))
            {
                if (currentStatistics.CharacterUsageList.Find(e => e.CharacterID == entry.Key) != null)
                {
                    int index = currentStatistics.CharacterUsageList.FindIndex(e => e.CharacterID == entry.Key);
                    characterEntry.WavesCompleted = currentStatistics.CharacterUsageList[index].WavesCompleted;
                }
                else
                {
                    characterEntry.WavesCompleted = 0;
                }

            }
            currentStatistics.CharacterUsageList.Add(characterEntry);
        }

        currentStatistics.WeaponUsageList.Clear();
        foreach (var entry in weaponUsageDict)
        {
            WeaponUsageEntry weaponEntry = new WeaponUsageEntry { WeaponID = entry.Key, UsageInfo = entry.Value };

            if (weaponUsageDict.TryGetValue(entry.Key, out UsageInfo usageInfo))
            {
                if (currentStatistics.WeaponUsageList.Find(e => e.WeaponID == entry.Key) != null)
                {
                    int index = currentStatistics.WeaponUsageList.FindIndex(e => e.WeaponID == entry.Key);
                }
            }

            currentStatistics.WeaponUsageList.Add(weaponEntry);
        }
    }
    
    public int CalculateEndOfRunXP()
    {
        int kills = CurrentRunKills;
        int waves = CurrentWaveCompleted;
        int meat = CurrentMeatCollected;
        float averageViewerScore = currentStatistics.AverageViewerScoreInRun;

        int baseXP = (kills * killXPWeight) + (waves * waveXPWeight) + (meat * meatXPWeight);

        float viewerMultiplier = Mathf.Lerp(viewerScoreMinMultiplier, viewerScoreMaxMultiplier, averageViewerScore);

        int finalXP = Mathf.RoundToInt(baseXP * viewerMultiplier);

        return finalXP;
    }



    #endregion

}

[Serializable]
public class GameStatistics
{
    // Current run statistics
    public float CurrentRunDuration;
    public string MostUsedCardInRun;
    public int HighestComboInRun;
    public float PeakDamageInRun;
    public int TotalXPInRun;
    public string MostEffectiveWeaponInRun;

    // Highest values recorded in a single run
    public int MostWavesCompletedInARun;    // Most waves completed in one run
    public int MostKillsInARun;             // Most kills in a single run
    public int MostLevelUpsInARun;          // Most level-ups in a single run
    public int MostChestsCollectedInARun;   // Most chests collected in a single run
    public int MostMeatCollectedInARun;    // Most candy collected in a single run
    public float AverageViewerScoreInRun;

    // Cumulative stats across all runs
    public int TotalWavesCompleted;   // Total waves completed
    public int TotalKills;                  // Total kills across all runs
    public int TotalDeaths;                 // Total deaths across all runs
    public int TotalMeatCollected;         // Total candy collected across all runs
    public float TotalPlayTime;               // Total time played
    public int TotalChestCollected;          //Total chest collected across all runs
    public float HighestAverageViewerScore;

  // Replace dictionaries with lists
    public List<CharacterUsageEntry> CharacterUsageList = new List<CharacterUsageEntry>();
    public List<WeaponUsageEntry> WeaponUsageList = new List<WeaponUsageEntry>();


}

[Serializable]
public class CharacterUsageEntry
{
    public string CharacterID;
    public UsageInfo UsageInfo;
    public int WavesCompleted;
}

[Serializable]
public class WeaponUsageEntry
{
    public string WeaponID;
    public UsageInfo UsageInfo;
    public float HighestDamageDealt;
    public int TimesUsed;
}

[Serializable]
public class UsageInfo
{
    public int UsageCount;
    public DateTime LastUsed;
}