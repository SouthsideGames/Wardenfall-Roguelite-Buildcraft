using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatContainerManager : MonoBehaviour
{
    public static StatContainerManager instance;

    [Header("ELEMENTS:")]
    [SerializeField] private StatContainerUI statContainerUI;

    private void Awake() 
    {
        if(instance == null)
           instance = this;
        else
            Destroy(gameObject);
    }

    private void GenerateContainers(Dictionary<Stat, float> _statDictionary, Transform _parent)
    {
        
        foreach(KeyValuePair<Stat, float> kvp in _statDictionary)
        {
            StatContainerUI statContainer = Instantiate(statContainerUI, _parent);

            Sprite icon = ResourceManager.GetStatIcon(kvp.Key);
            string statName = Enums.FormatStatName(kvp.Key);
            string statValue = kvp.Value.ToString();    

            statContainer.Configure(icon, statName, statValue); 
        }
    }

    public static void GenerateStatContainers(Dictionary<Stat, float> _statDictionary, Transform _parent)
    {
        instance.GenerateContainers(_statDictionary, _parent);   
    }
}
