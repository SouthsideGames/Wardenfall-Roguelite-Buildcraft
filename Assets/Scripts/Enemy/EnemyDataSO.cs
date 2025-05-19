using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Enemy Data", menuName = "Scriptable Objects/New Enemy Data", order = 1)]
public class EnemyDataSO : ScriptableObject
{
   
    [field: SerializeField] public string ID { get; private set; } 
    [field: SerializeField] public Sprite Icon { get; private set; }
    [field: SerializeField] public string Name { get; private set; }
    [field: SerializeField, TextArea] public string Description { get; private set; }
    [field: SerializeField] public EnemyType Type { get; private set; }

    public int contactDamage;
    public float detectionRadius;
    public int maxHealth;

}
