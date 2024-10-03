using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.TextCore.Text;

public class StatsManager : MonoBehaviour
{
    public static StatsManager Instance;

    [Header("DATA:")]
    [SerializeField] private CharacterDataSO characterData;

    [Header("SETTINGS:")]
    private Dictionary<Stat, float> addends = new Dictionary<Stat, float>();  
    private Dictionary<Stat, float> stats = new Dictionary<Stat, float>();  

    private void Awake()
    {
        if(Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        stats = characterData.BaseStats;

        foreach(KeyValuePair<Stat, float> kvp in stats)
            addends.Add(kvp.Key,0);
    }

    // Start is called before the first frame update
    void Start()
    {
        UpdateStats();
    }


    public void AddStat(Stat _stat, float _value)
    {
        // Character -> Base Stats

        if(addends.ContainsKey(_stat))
            addends[_stat] += _value;
        else
            Debug.LogError($"The key {_stat} has not been found");

        UpdateStats();

        //Objects -> List Object stats
    }

    private void UpdateStats()
    {
        IEnumerable<IStats> stats = 
            FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None)
            .OfType<IStats>();   

        foreach(IStats stat in stats) 
           stat.UpdateStats(this);

    }

    public float GetStatValue(Stat _stat)
    {
        float value = stats[_stat] + addends[_stat];
        return value;
    }
}
