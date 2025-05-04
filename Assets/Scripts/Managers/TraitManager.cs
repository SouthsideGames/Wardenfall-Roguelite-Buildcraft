using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TraitManager : MonoBehaviour
{
    public static TraitManager Instance;

    private Dictionary<string, int> traitStacks = new(); 
    [SerializeField] private List<TraitDataSO> allTraits;
    public List<TraitDataSO> AllTraits => allTraits;

    private Dictionary<string, int> suppressedBackup = null;

    private void Awake()
    {
        if(Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void AddTrait(string traitID) 
    {
        if (traitStacks.ContainsKey(traitID))
            traitStacks[traitID]++;
        else
            traitStacks[traitID] = 1;
    }

    public bool HasTrait(string traitID) => traitStacks.ContainsKey(traitID);

    public TraitTier GetActiveTier(string traitID) 
    {
        var trait = AllTraits.Find(t => t.TraitID == traitID);
        if (trait == null || !traitStacks.ContainsKey(traitID)) return null;
        return trait.GetTier(traitStacks[traitID]);
    }

    public List<(TraitDataSO trait, int stack)> GetAllActiveTraits() => traitStacks.Select(pair => (AllTraits.Find(t => t.TraitID == pair.Key), pair.Value)).ToList();

    public int GetStackCount(string traitID)
    {
        if (traitStacks.ContainsKey(traitID))
            return traitStacks[traitID];
        return 0;
    }

    public void DisableAllTraitsTemporarily()
    {
        if (suppressedBackup == null)
        {
            suppressedBackup = new Dictionary<string, int>(traitStacks);
            traitStacks.Clear();
            Debug.Log("All traits temporarily disabled.");
        }
    }

    public void RestoreAllSuppressedTraits()
    {
        if (suppressedBackup != null)
        {
            traitStacks = new Dictionary<string, int>(suppressedBackup);
            suppressedBackup = null;
            Debug.Log("All suppressed traits restored.");
        }
    }

    public int GetActiveTraitCount()
    {
        return traitStacks.Count;
    }
} 
