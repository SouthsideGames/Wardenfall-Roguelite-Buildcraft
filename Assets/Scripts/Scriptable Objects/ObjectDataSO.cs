using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

[CreateAssetMenu(fileName = "Object Data", menuName = "Scriptable Objects/New Object Data", order = 3)]
public class ObjectDataSO : ScriptableObject
{
    [field: Header("OBJECT:")]
    [field: Space(10)]
    [field: SerializeField] public Sprite Icon { get; private set; }
    [field: SerializeField] public string Name { get; private set; }
    [field: SerializeField] public int Price { get; private set; }
    [field: SerializeField] public int RecyclePrice { get; private set; }

    [field: Range(0,3)]
    [field: SerializeField] public int Rarity { get; private set; }


    [Header("DATA:")]
    [Space(10)]
    [SerializeField] private StatData[] statDatas;

    public Dictionary<Stat, float> BaseStats
    {
        get
        {
            Dictionary<Stat, float> stats = new Dictionary<Stat, float>();

            foreach (StatData data in statDatas)
                stats.Add(data.stat, data.value);

            return stats;
            
        }

        private set { }
    }

}

[System.Serializable]
public struct StatData
{
    public Stat stat;
    public float value;
}
