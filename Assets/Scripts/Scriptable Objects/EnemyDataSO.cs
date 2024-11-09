using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Enemy Data", menuName = "Scriptable Objects/New Enemy Data", order = 0)]
public class EnemyDataSO : ScriptableObject
{
    [field: Header("ENEMY DETAILS:")]
    [field: Space(10)]
    [field: SerializeField] public Sprite Icon { get; private set; }
    [field: SerializeField] public string Name { get; private set; }
    [field: SerializeField, TextArea] public string Description { get; private set; }

}
