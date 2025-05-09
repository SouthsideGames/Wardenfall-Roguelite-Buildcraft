using UnityEngine;
using TMPro;

public class StageCompletionStatsUI : MonoBehaviour
{
    [Header("Stats Text")]
    [SerializeField] private TextMeshProUGUI wavesCompletedText;
    [SerializeField] private TextMeshProUGUI killsText;
    [SerializeField] private TextMeshProUGUI runDurationText;
    [SerializeField] private TextMeshProUGUI peakDamageText;
    [SerializeField] private TextMeshProUGUI bestWeaponText;
    [SerializeField] private TextMeshProUGUI highestComboText;

    public void UpdateStats()
    {
        var stats = StatisticsManager.Instance.currentStatistics;
        
        wavesCompletedText.text = $"Waves Completed: {StatisticsManager.Instance.CurrentWaveCompleted}";
        killsText.text = $"Total Kills: {StatisticsManager.Instance.CurrentRunKills}";
        runDurationText.text = $"Stage Duration: {FormatTime(stats.CurrentRunDuration)}";
        peakDamageText.text = $"Peak Damage: {stats.PeakDamageInRun:F0}";
        bestWeaponText.text = $"Best Weapon: {stats.MostEffectiveWeaponInRun}";
        highestComboText.text = $"Highest Combo: {stats.HighestComboInRun}";
    }

    private string FormatTime(float seconds)
    {
        int minutes = (int)(seconds / 60);
        int remainingSeconds = (int)(seconds % 60);
        return $"{minutes:00}:{remainingSeconds:00}";
    }
}
