using UnityEngine;
using TMPro;

public class StageCompletionStatsUI : MonoBehaviour
{
     [Header("Run Stats")]
    [SerializeField] private TextMeshProUGUI wavesCompletedText;
    [SerializeField] private TextMeshProUGUI killsText;
    [SerializeField] private TextMeshProUGUI runDurationText;
    [SerializeField] private TextMeshProUGUI mostUsedCardText;
    [SerializeField] private TextMeshProUGUI meatCollectedText;
    [SerializeField] private TextMeshProUGUI peakDamageText;
    [SerializeField] private TextMeshProUGUI chestsOpenedText;
    [SerializeField] private TextMeshProUGUI bestWeaponText;
    [SerializeField] private TextMeshProUGUI highestComboText;
    [SerializeField] private TextMeshProUGUI totalXPText;

     public void UpdateStats()
    {
        var stats = StatisticsManager.Instance.currentStatistics;
        
        wavesCompletedText.text = $"Waves Completed: {StatisticsManager.Instance.CurrentWaveCompleted}";
        killsText.text = $"Total Kills: {StatisticsManager.Instance.CurrentRunKills}";
        runDurationText.text = $"Run Duration: {FormatTime(stats.CurrentRunDuration)}";
        mostUsedCardText.text = $"Most Used Card: {stats.MostUsedCardInRun}";
        meatCollectedText.text = $"Meat Collected: {StatisticsManager.Instance.CurrentMeatCollected}";
        peakDamageText.text = $"Peak Damage: {stats.PeakDamageInRun:F0}";
        chestsOpenedText.text = $"Chests Opened: {StatisticsManager.Instance.CurrentChestCollected}";
        bestWeaponText.text = $"Best Weapon: {stats.MostEffectiveWeaponInRun}";
        highestComboText.text = $"Highest Combo: {stats.HighestComboInRun}";
        totalXPText.text = $"Total XP: {stats.TotalXPInRun}";
    }

    private string FormatTime(float seconds)
    {
        int minutes = (int)(seconds / 60);
        int remainingSeconds = (int)(seconds % 60);
        return $"{minutes:00}:{remainingSeconds:00}";
    }
}
