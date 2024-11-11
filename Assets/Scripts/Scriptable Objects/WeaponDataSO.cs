using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu(fileName = "Weapon Data", menuName = "Scriptable Objects/New Weapon Data", order = 0)]
public class WeaponDataSO : ScriptableObject
{
    [field: Header("WEAPON DETAILS:")]
    [field: Space(10)]
    [field: SerializeField] public string ID { get; private set; } 
    [field: SerializeField] public Sprite Icon { get; private set; }
    [field: SerializeField] public string Name { get; private set; }
    [field: SerializeField, TextArea] public string Description { get; private set; }
    [field: SerializeField] public int PurchasePrice { get; private set; }   
    [field: SerializeField] public int RecyclePrice { get; private set; }   
    [field: SerializeField] public Weapon Prefab {get; private set;}

    [field: SerializeField] public AudioClip AttackSound { get; private set; }

    [Header("STATS:")]
    [Space(10)]
    [SerializeField, Tooltip("Base attack damage.")] private float attack;
    [SerializeField,  Tooltip("Attack speed of the character.")] private float attackSpeed;
    [SerializeField,  Tooltip("Chance to hit critically (%).")] private float criticalChance;
    [SerializeField,  Tooltip("Extra damage for critical hits.")] private float criticalDamageAmount;
    [SerializeField, Tooltip("Attack or ability range.")] private float range;

    public Dictionary<Stat, float> BaseStats
    {
        get 
        {
            return new Dictionary<Stat, float>
            {
                {Stat.Attack,       attack},
                {Stat.AttackSpeed,  attackSpeed},
                {Stat.CritChance,   criticalChance},
                {Stat.CritDamage,   criticalDamageAmount},
                {Stat.Range,        range},

            };
        }

        private set{}
    }

    public float GetStatValue(Stat _characterStat)
    {
        foreach(KeyValuePair<Stat, float> kvp in BaseStats)
            if(kvp.Key == _characterStat)
                return kvp.Value;   

        Debug.LogError("Stat not found..");
        return 0;
    }
}
