using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SouthsideGames.SaveManager;
using System.Linq;

public class CharacterEquipment : MonoBehaviour, IWantToBeSaved
{
    public List<EquippedBooster> equippedBoosters = new List<EquippedBooster>();
    private const string BOOSTER_SAVE_KEY = "equipped_boosters";

    [SerializeField] private CanvasGroup loadingBoostersUI;

    private void Awake()
    {
        LoadEquippedBoosters();
    }

    public void AssignBoosterToSlot(int slotIndex, StatBoosterSO booster, Stat chosenStat)
    {
        if (booster == null)
        {
            Debug.LogWarning("Cannot assign null booster.");
            return;
        }

        // Prevent duplicate equip
        for (int i = 0; i < equippedBoosters.Count; i++)
        {
            if (i == slotIndex) continue; // Skip current slot

            if (equippedBoosters[i].booster == booster)
            {
                Debug.LogWarning($"Booster '{booster.name}' is already equipped in slot {i}. Cannot equip twice.");
                return;
            }
        }

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
            else
            {
                serialized.Add("null");
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

        if (string.IsNullOrEmpty(raw))
            return;

        StartCoroutine(WaitForBoosterRegistryAndLoad(raw));
    }

    private IEnumerator WaitForBoosterRegistryAndLoad(string raw)
    {
        if (loadingBoostersUI != null)
        {
            loadingBoostersUI.alpha = 1f;
            loadingBoostersUI.gameObject.SetActive(true);
        }

        while (ProgressionBoosterRegistry.Instance == null || ProgressionBoosterRegistry.Instance.allBoosters.Count == 0)
            yield return null;

        string[] entries = raw.Split(',');

        foreach (var entry in entries)
        {
            if (entry == "null")
            {
                equippedBoosters.Add(new EquippedBooster());
                continue;
            }

            var parts = entry.Split(':');
            if (parts.Length != 2) continue;

            string boosterID = parts[0];
            if (!int.TryParse(parts[1], out int statIndex)) continue;

            var booster = ProgressionBoosterRegistry.Instance.allBoosters.Find(b => b.boosterID == boosterID);
            equippedBoosters.Add(new EquippedBooster
            {
                booster = booster,
                chosenStat = (Stat)statIndex
            });
        }

        var stats = GetComponent<CharacterStats>();
        if (stats != null)
        {
            // Clear all previous stat boosts
            foreach (Stat stat in System.Enum.GetValues(typeof(Stat)))
                stats.RevertBoost(stat);

            // Re-apply all equipped boosters
            stats.ApplyProgressionBoosters(equippedBoosters);
        }

        if (loadingBoostersUI != null)
        {
            LeanTween.alphaCanvas(loadingBoostersUI, 0f, 0.5f)
                .setOnComplete(() => loadingBoostersUI.gameObject.SetActive(false));
        }
    }

    public bool CanEquipBooster(StatBoosterSO booster)
    {
        if (booster == null) return false;
        return !equippedBoosters.Any(b => b.booster == booster);
    }

    public void Save() => SaveEquippedBoosters();
    public void Load() => LoadEquippedBoosters();
}

[System.Serializable]
public class EquippedBooster
{
    public StatBoosterSO booster;
    public Stat chosenStat;
}
