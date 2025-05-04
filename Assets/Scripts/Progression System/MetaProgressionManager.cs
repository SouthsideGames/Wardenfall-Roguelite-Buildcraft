using UnityEngine;
using SouthsideGames.SaveManager;
using System;
using System.Collections.Generic;

public class MetaProgressionManager : MonoBehaviour, IGameStateListener
{
    public static MetaProgressionManager Instance;

    public int MetaXP { get; private set; }
    public int UnlockPoints { get; private set; }
    public int LastGainedXP { get; private set; }
    public int PlayerLevel { get; private set; }

    private const string XP_KEY = "meta_xp";
    private const string POINTS_KEY = "unlock_points";
    private const string LEVEL_KEY = "player_level";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Load();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddMetaXP(int amount)
    {
        MetaXP += amount;
        LastGainedXP = amount;

        int newPoints = Mathf.FloorToInt(amount / 100f);
        UnlockPoints += newPoints;

        int xpForLevel = 1000;
        if (MetaXP >= (PlayerLevel + 1) * xpForLevel)
            PlayerLevel++;

        Save();
    }

    private void Save()
    {
        SaveManager.GameData.Add(XP_KEY, typeof(int), MetaXP.ToString());
        SaveManager.GameData.Add(POINTS_KEY, typeof(int), UnlockPoints.ToString());
        SaveManager.GameData.Add(LEVEL_KEY, typeof(int), PlayerLevel.ToString());
    }

    private void Load()
    {
        if (SaveManager.GameData.TryGetValue(XP_KEY, out var _, out var xpStr))
            MetaXP = int.Parse(xpStr);

        if (SaveManager.GameData.TryGetValue(POINTS_KEY, out var _, out var pointsStr))
            UnlockPoints = int.Parse(pointsStr);

        if (SaveManager.GameData.TryGetValue(LEVEL_KEY, out var _, out var levelStr))
            PlayerLevel = int.Parse(levelStr);
    }

    // Optional: Reset LastGainedXP when you no longer want to display it
    public void ClearLastGainedXP()
    {
        LastGainedXP = 0;
    }

    // Listen for GameState changes (optional behavior if needed here)
    public void GameStateChangedCallback(GameState newState)
    {
        // Optional: You can react to specific states here
        // Example: Reset XP display when state changes away
        if (newState != GameState.CharacterProgression)
            ClearLastGainedXP();
    }
}
