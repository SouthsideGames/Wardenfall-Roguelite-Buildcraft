using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class SummaryContainerUI : MonoBehaviour
{
    // Existing fields
    [Header("Cumulative Stats")]
    [SerializeField] private TextMeshProUGUI totalKillsText;
    [SerializeField] private TextMeshProUGUI totalDeathsText;
    [SerializeField] private TextMeshProUGUI totalMeatText;
    [SerializeField] private TextMeshProUGUI totalChestText;
    [SerializeField] private TextMeshProUGUI totalWavesCompleted;
    [SerializeField] private TextMeshProUGUI totalPlayTime;

    [Header("Single-Run Bests")]
    [SerializeField] private TextMeshProUGUI mostWavesCompleted;
    [SerializeField] private TextMeshProUGUI mostKillsText;
    [SerializeField] private TextMeshProUGUI mostLevelUpsText;
    [SerializeField] private TextMeshProUGUI mostChestsText;
    [SerializeField] private TextMeshProUGUI mostMeatText;
    [SerializeField] private TextMeshProUGUI fastestRunTimeText;
    [SerializeField] private TextMeshProUGUI highestAverageViewerScoreText;

    [Header("Last Run Stats")]
    [SerializeField] private TextMeshProUGUI mostUsedCardText;
    [SerializeField] private TextMeshProUGUI mostEffectiveWeaponText;
    [SerializeField] private TextMeshProUGUI peakDamageText;
    [SerializeField] private TextMeshProUGUI totalXPInRunText;

    private void Start()
    {
        UpdateStatsUI();
    }

    public void UpdateStatsUI()
    {
        GameStatistics stats = StatisticsManager.Instance.currentStatistics;

        // ---- CUMULATIVE STATS ----
        float totalPlayTimeInSeconds = stats.TotalPlayTime;
        TimeSpan timeSpan = TimeSpan.FromSeconds(totalPlayTimeInSeconds);
        string formattedTotalTime = string.Format("{0:D2}:{1:D2}:{2:D2}",
            timeSpan.Hours + (timeSpan.Days * 24),
            timeSpan.Minutes,
            timeSpan.Seconds);

        totalWavesCompleted.text = $"Total Waves Completed: {stats.TotalWavesCompleted}";
        totalKillsText.text = $"Total Kills: {stats.TotalKills}";
        totalDeathsText.text = $"Total Deaths: {stats.TotalDeaths}";
        totalMeatText.text = $"Total Candy Collected: {stats.TotalMeatCollected}";
        totalPlayTime.text = $"Total Play Time: {formattedTotalTime}";
        totalChestText.text = $"Total Chest Collected: {stats.TotalChestCollected}";

        // ---- SINGLE-RUN BESTS ----
        mostWavesCompleted.text = $"Most Waves Completed in a Run: {stats.MostWavesCompletedInARun}";
        mostKillsText.text = $"Most Kills in a Run: {stats.MostKillsInARun}";
        mostLevelUpsText.text = $"Most Level-Ups in a Run: {stats.MostLevelUpsInARun}";
        mostChestsText.text = $"Most Chests Collected in a Run: {stats.MostChestsCollectedInARun}";
        mostMeatText.text = $"Most Candy Collected in a Run: {stats.MostMeatCollectedInARun}";

        // Fastest Run Time
        if (stats.FastestRunTime > 0)
        {
            TimeSpan fastestTimeSpan = TimeSpan.FromSeconds(stats.FastestRunTime);
            string fastestFormatted = string.Format("{0:D2}:{1:D2}:{2:D2}",
                fastestTimeSpan.Hours + (fastestTimeSpan.Days * 24),
                fastestTimeSpan.Minutes,
                fastestTimeSpan.Seconds);
            fastestRunTimeText.text = $"Fastest Run Time: {fastestFormatted}";
        }
        else
        {
            fastestRunTimeText.text = "Fastest Run Time: N/A";
        }

        highestAverageViewerScoreText.text = $"Highest Average Viewer Score: {stats.HighestAverageViewerScore:F2}";

        // ---- LAST RUN STATS ----
        mostUsedCardText.text = $"Most Used Card (Last Run): {stats.MostUsedCardInRun ?? "N/A"}";
        mostEffectiveWeaponText.text = $"Most Effective Weapon (Last Run): {stats.MostEffectiveWeaponInRun ?? "N/A"}";
        peakDamageText.text = $"Peak Damage (Last Run): {stats.PeakDamageInRun:F2}";
        totalXPInRunText.text = $"Total XP Earned (Last Run): {stats.TotalXPInRun}";
    }
}
