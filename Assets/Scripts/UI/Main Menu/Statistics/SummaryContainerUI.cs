using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class SummaryContainerUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI totalKillsText;
    [SerializeField] private TextMeshProUGUI totalDeathsText;
    [SerializeField] private TextMeshProUGUI totalCandyText;
    [SerializeField] private TextMeshProUGUI totalChestText;
    [SerializeField] private TextMeshProUGUI totalWavesCompleted;
    [SerializeField] private TextMeshProUGUI totalPlayTime;
    [SerializeField] private TextMeshProUGUI mostWavesCompleted;
    [SerializeField] private TextMeshProUGUI mostKillsText;
    [SerializeField] private TextMeshProUGUI mostLevelUpsText;
    [SerializeField] private TextMeshProUGUI mostChestsText;
    [SerializeField] private TextMeshProUGUI mostCandyText;


    private void Start()
    {
        // Initialize and update the UI with the saved stats
        UpdateStatsUI();
    }

    public void UpdateStatsUI()
    {
        GameStatistics stats = StatisticsManager.Instance.currentStatistics;

           // Fetch the total play time from StatsManager
        float totalPlayTimeInSeconds = StatisticsManager.Instance.currentStatistics.TotalPlayTime;

        // Convert total play time from seconds to TimeSpan
        TimeSpan timeSpan = TimeSpan.FromSeconds(totalPlayTimeInSeconds);

         // Format the time as hours:minutes:seconds (e.g., "02:30:15")
        string formattedTime = string.Format("{0:D2}:{1:D2}:{2:D2}",
            timeSpan.Hours + (timeSpan.Days * 24),  // Add hours for multi-day playtime
            timeSpan.Minutes,
            timeSpan.Seconds);

        totalWavesCompleted.text = $"Total Waves Completed: {stats.TotalWavesCompleted}";
        totalKillsText.text = $"Total Kills: {stats.TotalKills}";
        totalDeathsText.text = $"Total Deaths: {stats.TotalDeaths}";
        totalCandyText.text = $"Total Candy Collected: {stats.TotalCandyCollected}";
        totalPlayTime.text = $"Total Play Time: {formattedTime}";
        totalChestText.text = $"Total Chest Collected: {stats.TotalChestCollected}";

        mostWavesCompleted.text = $"Most Waves Completed in a Run: {stats.MostWavesCompletedInARun}";
        mostKillsText.text = $"Most Kills in a Run: {stats.MostKillsInARun}";
        mostLevelUpsText.text = $"Most Level-Ups in a Run: {stats.MostLevelUpsInARun}";
        mostChestsText.text = $"Most Chests Collected in a Run: {stats.MostChestsCollectedInARun}";
        mostCandyText.text = $"Most Candy Collected in a Run: {stats.MostCandyCollectedInARun}";

    }
}
