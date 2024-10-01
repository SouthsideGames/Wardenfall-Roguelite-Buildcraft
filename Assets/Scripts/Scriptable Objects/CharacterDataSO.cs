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
    [SerializeField, Tooltip("Base attack damage.")] private float attack;
    [SerializeField,  Tooltip("Attack speed of the character.")] private float attackSpeed;
    [SerializeField,  Tooltip("Chance to hit critically (%).")] private float criticalChance;
    [SerializeField,  Tooltip("Extra damage for critical hits (%).")] private float criticalPercent;
    [SerializeField, Tooltip("Character movement speed.")] private float moveSpeed;
    [SerializeField, Tooltip("Maximum health points.")] private float maxHealth;
    [SerializeField, Tooltip("Attack or ability range.")] private float range;
    [SerializeField, Tooltip("Health regenerated per second.")] private float healthRecoverySpeed;
    [SerializeField, Tooltip("Amount of health to regenerate.")] private float healthRecoveryValue;
    [SerializeField, Tooltip("Damage reduction value.")] private float armor;
    [SerializeField, Tooltip("Increase the chance of chest spawning.")] private float luck;
    [SerializeField, Tooltip("Chance to dodge attacks (%).")] private float dodge;
    [SerializeField, Tooltip("Damage converted to health (%).")] private float lifeSteal;
    [SerializeField, Tooltip("Reduces enemy crit (%).")] private float criticalResistancePercent;
    [SerializeField, Tooltip("Radius for auto item pickup.")] private float pickupRange;

    public Dictionary<CharacterStat, float> BaseStats
    {
        get 
        {
            return new Dictionary<CharacterStat, float>
            {
                {CharacterStat.Attack, attack},
                {CharacterStat.AttackSpeed, attackSpeed},
                {CharacterStat.CriticalChance, criticalChance},
                {CharacterStat.CriticalPercent, criticalPercent},
                {CharacterStat.MoveSpeed, moveSpeed},
                {CharacterStat.MaxHealth, maxHealth},
                {CharacterStat.Range, range},
                {CharacterStat.HealthRecoverySpeed, healthRecoverySpeed},
                {CharacterStat.HealthRecoveryValue, healthRecoveryValue},
                {CharacterStat.Armor, armor},
                {CharacterStat.Luck, luck},
                {CharacterStat.Dodge, dodge},
                {CharacterStat.LifeSteal, lifeSteal},
                {CharacterStat.CriticalResistancePercent, criticalResistancePercent},
                {CharacterStat.PickupRange, pickupRange},   

            };
        }

        private set{}
    }
}
