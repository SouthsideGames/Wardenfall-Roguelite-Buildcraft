using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CharacterStatsManager : MonoBehaviour
{
    public static CharacterStatsManager Instance;

    [Header("SETTINGS:")]
    private Dictionary<CharacterStat, float> addends = new Dictionary<CharacterStat, float>();  

    private void Awake()
    {
        if(Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        addends.Add(CharacterStat.MaxHealth, 10);
        UpdateCharacterStats();
    }

    // Update is called once per frame
    void Update()
    {
        
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
        return addends[_characterStat];
    }
}
