using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO: update object code to limit the number of objects that can be held
public class CharacterObjects : MonoBehaviour
{
    [field: SerializeField] public List<ObjectDataSO> Objects { get; private set; }
    private CharacterStats characterStats;

    private void Awake()
    {
        characterStats = GetComponent<CharacterStats>();        
    }

    private void Start()
    {
        foreach (ObjectDataSO objectData in Objects)
            characterStats.AddObject(objectData.BaseStats);
    }

    public void AddObject(ObjectDataSO _objectToAdd)
    {
        Objects.Add(_objectToAdd);

        characterStats.AddObject(_objectToAdd.BaseStats);
    }
}
