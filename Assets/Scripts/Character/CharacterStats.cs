using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using System.Collections;

public class CharacterStats : MonoBehaviour
{
    public Action OnDataStored;

    [Header("DATA:")]
    [SerializeField] private CharacterDataSO characterData;
    public CharacterDataSO CharacterData => characterData;

    [Header("SETTINGS:")]
    private Dictionary<Stat, float> addends = new();
    private Dictionary<Stat, float> stats = new();
    private Dictionary<Stat, float> objectAddends = new();
    private Dictionary<Stat, float> boostedStats = new();
    private readonly List<IStats> statReceivers = new();
    
    private bool critOnlyMode = false;
    private float critOnlyMultiplier = 3f;

    private void Awake()
    {
        CharacterSelectionManager.OnCharacterSelected += CharacterSelectedCallback;

        stats = characterData.BaseStats;

        foreach (Stat stat in Enum.GetValues(typeof(Stat)))
        {
            addends.TryAdd(stat, 0f);
            objectAddends.TryAdd(stat, 0f);
            boostedStats.TryAdd(stat, 0f);
        }
    }

    private void OnDestroy() => CharacterSelectionManager.OnCharacterSelected -= CharacterSelectedCallback;

    private void Start() => UpdateStats();

    public void AddStat(Stat stat, float value)
    {
        if (addends.ContainsKey(stat))
            addends[stat] += value;

        UpdateStats();
    }

    public float GetStatValue(Stat stat)
    {
        float baseValue = stats.TryGetValue(stat, out float baseStat) ? baseStat : 0f;
        float additive = addends.TryGetValue(stat, out float add) ? add : 0f;
        float objectBonus = objectAddends.TryGetValue(stat, out float objAdd) ? objAdd : 0f;
        float boosterBonus = boostedStats.TryGetValue(stat, out float boost) ? boost : 0f;

        float total = baseValue + additive + objectBonus + boosterBonus;
        return Mathf.Max(1, total);
    }

    public void AddObject(Dictionary<Stat, float> objectStats)
    {
        foreach (var kvp in objectStats)
            objectAddends[kvp.Key] += kvp.Value;

        UpdateStats();
    }

    public void RemoveObjectStats(Dictionary<Stat, float> objectStats)
    {
        foreach (var kvp in objectStats)
            objectAddends[kvp.Key] -= kvp.Value;

        UpdateStats();
    }

    private void CharacterSelectedCallback(CharacterDataSO selectedData)
    {
        OnDataStored?.Invoke();
        characterData = selectedData;
        stats = characterData.BaseStats;

        CharacterEquipment equipment = GetComponent<CharacterEquipment>();
        if (equipment != null)
            ApplyProgressionBoosters(equipment.equippedBoosters);

        UpdateStats();


        if (ChallengeManager.IsActive(ChallengeMode.RogueRoulette))
        {
            ChallengeManager.Instance.ApplyWaveRouletteEffect(this);
        }
    }


    public void BoostStat(Stat stat, float value)
    {
        if (boostedStats.ContainsKey(stat))
        {
            boostedStats[stat] += value;
            UpdateStats();
        }
    }

    public void RevertBoost(Stat stat)
    {
        if (boostedStats.ContainsKey(stat))
        {
            boostedStats[stat] = 0f;
            UpdateStats();
        }
    }

    public void ApplyProgressionBoosters(List<EquippedBooster> boosters)
    {
        foreach (var equipped in boosters)
        {
            if (equipped.booster != null)
                BoostStat(equipped.chosenStat, equipped.booster.bonusValue);
        }
    }

    public void RegisterStatReceiver(IStats receiver)
    {
        if (!statReceivers.Contains(receiver))
            statReceivers.Add(receiver);
    }

    public void UnregisterStatReceiver(IStats receiver)
    {
        if (statReceivers.Contains(receiver))
            statReceivers.Remove(receiver);
    }

    private void UpdateStats()
    {
        foreach (var receiver in statReceivers)
        {
            receiver.UpdateWeaponStats(this);
        }
    }

    public void ApplyTemporaryModifier(Stat stat, float multiplier, float duration)
    {
        StartCoroutine(ApplyModifierRoutine(stat, multiplier, duration));
    }

    private IEnumerator ApplyModifierRoutine(Stat stat, float multiplier, float duration)
    {
        float current = GetStatValue(stat);
        float target = current * multiplier;
        float delta = target - current;

        // Apply boost
        BoostStat(stat, delta);

        Debug.Log($"[RogueRoulette] {stat} modified by x{multiplier}");

        yield return new WaitForSeconds(duration);

        // Remove boost
        RevertBoost(stat);
    }
    
    
    public bool IsCritOnlyMode() => critOnlyMode;
    public float GetCritOnlyMultiplier() => critOnlyMultiplier;

    public void EnableCritOnlyMode(float multiplier)
    {
        critOnlyMode = true;
        critOnlyMultiplier = multiplier;
    }

}
