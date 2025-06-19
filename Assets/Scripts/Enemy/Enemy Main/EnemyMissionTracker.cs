using UnityEngine;
using System;
using SouthsideGames.DailyMissions;

[RequireComponent(typeof(Enemy))]
public class EnemyMissionTracker : MonoBehaviour
{
    private Enemy enemy;
    private static float multiKillWindow = 0.5f;
    private static float lastKillTime;
    private static int killChain;

    private void Awake()
    {
        enemy = GetComponent<Enemy>();
    }

    public void ReportKill()
    {
        float sinceLast = Time.time - lastKillTime;

        if (sinceLast <= multiKillWindow)
        {
            killChain++;
            if (killChain >= 3)
            {
                MissionManager.Increment(MissionType.multiKills, 1);
                MissionManager.Increment(MissionType.multiKills2, 1);
                MissionManager.Increment(MissionType.multiKills3, 1);
                MissionManager.Increment(MissionType.multiKills4, 1);
                WaveManager.Instance?.AdjustViewerScore(0.15f);
                killChain = 0;
            }
        }
        else
        {
            killChain = 1;
        }

        lastKillTime = Time.time;
        WaveManager.Instance?.ReportKill();
        IncrementTotals();
    }

    private void IncrementTotals()
    {
        MissionManager.Increment(MissionType.eliminate100Enemies, 1);
        MissionManager.Increment(MissionType.eliminate500Enemies, 1);
        MissionManager.Increment(MissionType.eliminate1000Enemies, 1);
        MissionManager.Increment(MissionType.eliminate2000Enemies, 1);
    }
}
