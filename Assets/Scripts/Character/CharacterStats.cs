using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class CharacterStats : MonoBehaviour
{
    public static CharacterStats Instance;
    public Action OnDataStored;

    [Header("DATA:")]
    [SerializeField] private CharacterDataSO characterData;
    public CharacterDataSO CharacterData => characterData;

    [Header("SETTINGS:")]
    private Dictionary<Stat, float> addends = new Dictionary<Stat, float>();
    private Dictionary<Stat, float> stats = new Dictionary<Stat, float>();
    private Dictionary<Stat, float> objectAddends = new Dictionary<Stat, float>();
    private Dictionary<Stat, float> boostedStats = new Dictionary<Stat, float>();

    private void Awake()
    {
        CharacterSelectionManager.OnCharacterSelected += CharacterSelectedCallback;

        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        stats = characterData.BaseStats;

        foreach (KeyValuePair<Stat, float> kvp in stats)
        {
            addends.Add(kvp.Key, 0);
            objectAddends.Add(kvp.Key, 0);
            boostedStats.Add(kvp.Key, 0);
        }
    }

    private void OnDestroy() => CharacterSelectionManager.OnCharacterSelected -= CharacterSelectedCallback;

    void Start() => UpdateStats();

    public void AddStat(Stat _stat, float _value)
    {
        if (addends.ContainsKey(_stat))
            addends[_stat] += _value;
        else
            Debug.LogError($"The key {_stat} has not been found");

        UpdateStats();
    }

    private void UpdateStats()
    {
        IEnumerable<IStats> stats =
            FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include, FindObjectsSortMode.None)
            .OfType<IStats>();

        foreach (IStats stat in stats)
            stat.UpdateWeaponStats(this);
    }

    public float GetStatValue(Stat _stat)
    {
        
        float totalValue = stats[_stat] + addends[_stat] + objectAddends[_stat];
        return Mathf.Max(1, totalValue);
    }

    public void AddObject(Dictionary<Stat, float> _objectStats)
    {
        foreach (KeyValuePair<Stat, float> kvp in _objectStats)
            objectAddends[kvp.Key] += kvp.Value;

        UpdateStats();
    }

    public void RemoveObjectStats(Dictionary<Stat, float> _objectStats)
    {
        foreach (KeyValuePair<Stat, float> kvp in _objectStats)
            objectAddends[kvp.Key] -= kvp.Value;

        UpdateStats();
    }

    private void CharacterSelectedCallback(CharacterDataSO _characterData)
    {
        OnDataStored?.Invoke();
        characterData = _characterData;
        stats = characterData.BaseStats;

        UpdateStats();
    }

    public void BoostStat(Stat _stat, float _value)
    {
        if (boostedStats.ContainsKey(_stat))
        {
            float currentAddend = addends[_stat];
            boostedStats[_stat] = currentAddend;

            addends[_stat] += _value;
            UpdateStats();
        }
        else
        {
            Debug.LogError($"Stat {_stat} not found for boosting.");
        }
    }

    public void RevertBoost(Stat _stat)
    {
        if (boostedStats.ContainsKey(_stat))
        {
            addends[_stat] = boostedStats[_stat];
            boostedStats[_stat] = 0; 
            UpdateStats();
        }
        else
        {
            Debug.LogError($"Stat {_stat} not found for reverting boost.");
        }
    }

}
