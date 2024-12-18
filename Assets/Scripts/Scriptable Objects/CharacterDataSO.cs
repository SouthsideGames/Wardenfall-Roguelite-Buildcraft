using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

[CreateAssetMenu(fileName = "Character Data", menuName = "Scriptable Objects/New Character Data", order = 0)]
public class CharacterDataSO : ScriptableObject
{
    [field: SerializeField] public string ID { get; private set; } 
    [field: SerializeField] public Sprite Icon { get; private set; }
    [field: SerializeField] public string Name { get; private set; }
    [field: SerializeField, TextArea] public string Description { get; private set; }
    [field: SerializeField] public int PurchasePrice { get; private set; }   

    [HorizontalLine]
    [SerializeField, Tooltip("Base attack damage.")] private float attack;
    [SerializeField,  Tooltip("Attack speed of the character.")] private float attackSpeed;
    [SerializeField, Range(1, 15), Tooltip("Chance to hit critically (%).")] private float critChance;
    [SerializeField, Range(1.1f, 3f), Tooltip("Extra damage for critical hits (%).")] private float critDamage;
    [SerializeField, Tooltip("Character movement speed.")] private float moveSpeed;
    [SerializeField, Tooltip("Maximum health points.")] private float maxHealth;
    [SerializeField, Tooltip("Attack or ability range.")] private float range;
    [SerializeField, Range(1.1f, 3f), Tooltip("Health regenerated per second.")] private float regenSpeed;
    [SerializeField, Tooltip("Amount of health to regenerate.")] private float regenValue;
    [SerializeField, Tooltip("Damage reduction value.")] private float armor; 
    [SerializeField, Tooltip("Increase the chance of chest spawning.")] private float luck;
    [SerializeField, Range(1.1f, 10f), Tooltip("Chance to dodge attacks (%).")] private float dodge;
    [SerializeField, Tooltip("Damage converted to health (%).")] private float lifeSteal;
    [SerializeField, Range(1.1f, 5f), Tooltip("Reduces enemy crit (%).")] private float critResist;
    [SerializeField, Tooltip("Radius for auto item pickup.")] private float pickupRange;

    public Dictionary<Stat, float> BaseStats
    {
        get 
        {
            return new Dictionary<Stat, float>
            {
                {Stat.Attack,          attack},
                {Stat.AttackSpeed,     attackSpeed},
                {Stat.CritChance,      critChance},
                {Stat.CritDamage,      critDamage},
                {Stat.MoveSpeed,       moveSpeed},
                {Stat.MaxHealth,       maxHealth},
                {Stat.Range,           range},
                {Stat.RegenSpeed,      regenSpeed},
                {Stat.RegenValue,      regenValue},
                {Stat.Armor,           armor},
                {Stat.Luck,            luck},
                {Stat.Dodge,           dodge},
                {Stat.LifeSteal,       lifeSteal},
                {Stat.CritResist,      critResist},
                {Stat.PickupRange,     pickupRange},   

            };
        }

        private set{}
    }

    public Dictionary<Stat, float> NonNeutralStats
    {
        get 
        {
            Dictionary<Stat, float> nonNeutralStats = new Dictionary<Stat, float>();

            foreach(KeyValuePair<Stat, float> kvp in BaseStats)
                if(kvp.Value != 0)
                   nonNeutralStats.Add(kvp.Key, kvp.Value);

                return nonNeutralStats;
        }

        private set {}
    }
}
