using System.Collections.Generic;
using UnityEngine;

 [CreateAssetMenu(fileName = "Enemy Trait Data", menuName = "Scriptable Objects/New Enemy Trait Data", order = 7)]
public class EnemyTraitDataSO : ScriptableObject
{
    [field: SerializeField] public string TraitID;
    [field: SerializeField] public List<EnemyTraitTier> Tiers;

    public EnemyTraitTier GetTier(int stackCount) {
        int index = Mathf.Clamp(stackCount - 1, 0, Tiers.Count - 1);
        return Tiers[index];
    }
}
