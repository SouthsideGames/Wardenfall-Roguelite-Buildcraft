using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

[CreateAssetMenu(fileName = "Character Data", menuName = "Scriptable Objects/New Character Data", order = 0)]
public class CharacterDataSO : ScriptableObject
{
    [field: SerializeField] public string name { get; private set; }
    [field: SerializeField] public Sprite Icon { get; private set; }

    [field: SerializeField] public int PurchasePrice { get; private set; }   

    [HorizontalLine]
    [SerializeField] private float attack;
    [SerializeField] private float attackSeed;
    [SerializeField] private float criticalChance;
    [SerializeField] private float criticalPercent;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float maxHealth;
    [SerializeField] private float range;
    [SerializeField] private float healthRecoverySpeed;
    [SerializeField] private float armor;
    [SerializeField] private float luck;
    [SerializeField] private float dodge;
    [SerializeField] private float lifeSteal;
    [SerializeField] private float criticalResistance;
    [SerializeField] private float pickupRange;

    public Dictionary<CharacterStat, float> BaseStats
    {
        get 
        {
            return new Dictionary<CharacterStat, float>
            {
                {CharacterStat.Attack, attack},
                {CharacterStat.AttackSpeed, attackSeed},
                {CharacterStat.CriticalChance, criticalChance},
                {CharacterStat.CriticalPercent, criticalPercent},
                {CharacterStat.MoveSpeed, moveSpeed},
                {CharacterStat.MaxHealth, maxHealth},
                {CharacterStat.Range, range},
                {CharacterStat.HealthRecoverySpeed, healthRecoverySpeed},
                {CharacterStat.Armor, armor},
                {CharacterStat.Luck, luck},
                {CharacterStat.Dodge, dodge},
                {CharacterStat.LifeSteal, lifeSteal},
                {CharacterStat.CriticalResistance, criticalResistance},
                {CharacterStat.PickupRange, pickupRange},   

            };
        }

        private set{}
    }
}
