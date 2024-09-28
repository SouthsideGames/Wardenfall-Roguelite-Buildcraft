using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.TextCore.Text;

public class CharacterStatsManager : MonoBehaviour
{
    public static CharacterStatsManager Instance;

    [Header("DATA:")]
    [SerializeField] private CharacterDataSO characterData;

    [Header("SETTINGS:")]
    private Dictionary<CharacterStat, float> addends = new Dictionary<CharacterStat, float>();  
    private Dictionary<CharacterStat, float> characterStats = new Dictionary<CharacterStat, float>();  

    private void Awake()
    {
        if(Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        characterStats = characterData.BaseStats;

        foreach(KeyValuePair<CharacterStat, float> kvp in characterStats)
            addends.Add(kvp.Key,0);
    }

    // Start is called before the first frame update
    void Start()
    {
        UpdateCharacterStats();
    }


    public void AddCharacterStat(CharacterStat _characterStat, float _value)
    {
        // Character -> Base Stats

        if(addends.ContainsKey(_characterStat))
            addends[_characterStat] += _value;
        else
            Debug.LogError($"The key {_characterStat} has not been found");

        UpdateCharacterStats();

        //Objects -> List Object stats
    }

    private void UpdateCharacterStats()
    {
        IEnumerable<ICharacterStats> characterStats = 
            FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None)
            .OfType<ICharacterStats>();   

        foreach(ICharacterStats characterStat in characterStats) 
           characterStat.UpdateStats(this);

    }

    public float GetStatValue(CharacterStat _characterStat)
    {
        float value = characterStats[_characterStat] + addends[_characterStat];
        return value;
    }
}
