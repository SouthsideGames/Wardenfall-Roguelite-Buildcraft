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
    [field: SerializeField] public bool HasEvolution { get; private set; }
    [field: SerializeField] public EnemyDataSO EvolutionData { get; private set; }
    [field: SerializeField] public GameObject EvolutionPrefab { get; private set; }


    [Header("BASE VALUES:")]
    public int contactDamage;
    public float detectionRadius = 6.0f;
    public int maxHealth;

    [Header("SPAWN VALUES:")]
    public float spawnSize = 1.2f;
    public float spawnTime = 0.3f;
    public int numberOfLoops = 4;


}
