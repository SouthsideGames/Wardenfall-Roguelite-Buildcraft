using System.Collections.Generic;
using UnityEngine;
using SouthsideGames.SaveManager;

[System.Serializable]
public class EquippedBooster
{
    public StatBoosterSO booster;
    public Stat chosenStat;
}

public class CharacterEquipment : MonoBehaviour, IWantToBeSaved
{
    public List<EquippedBooster> equippedBoosters = new List<EquippedBooster>();
    private const string BOOSTER_SAVE_KEY = "equipped_boosters";

    private void Awake()
    {
        LoadEquippedBoosters();
    }

    public void AssignBoosterToSlot(int slotIndex, StatBoosterSO booster, Stat chosenStat)
    {
        // Expand list if needed
        while (equippedBoosters.Count <= slotIndex)
        {
            equippedBoosters.Add(new EquippedBooster());
        }

        equippedBoosters[slotIndex].booster = booster;
        equippedBoosters[slotIndex].chosenStat = chosenStat;

        SaveEquippedBoosters();
    }

    public void SaveEquippedBoosters()
    {
        List<string> serialized = new();

        foreach (var booster in equippedBoosters)
        {
            if (booster.booster != null)
            {
                string line = $"{booster.booster.boosterID}:{(int)booster.chosenStat}";
                serialized.Add(line);
            }
        }

        string result = string.Join(",", serialized);
        SaveManager.GameData.Add(BOOSTER_SAVE_KEY, typeof(string), result);
    }

   public void LoadEquippedBoosters()
    {
        equippedBoosters.Clear();

        if (!SaveManager.GameData.TryGetValue(BOOSTER_SAVE_KEY, out var _, out var raw))
            return;

        if (string.IsNullOrEmpty(raw) || BoosterRegistry.Instance == null)
        {
            Debug.LogWarning("BoosterRegistry not initialized yet — delaying booster load.");
            return;
        }

        string[] entries = raw.Split(',');
        foreach (var entry in entries)
        {
            var parts = entry.Split(':');
            if (parts.Length != 2) continue;

            string boosterID = parts[0];
            int statIndex = int.Parse(parts[1]);

            var booster = BoosterRegistry.Instance.allBoosters.Find(b => b.boosterID == boosterID);
            if (booster != null)
            {
                equippedBoosters.Add(new EquippedBooster
                {
                    booster = booster,
                    chosenStat = (Stat)statIndex
                });
            }
        }
    }


    public void Save() => SaveEquippedBoosters();
    public void Load() => LoadEquippedBoosters();
}
