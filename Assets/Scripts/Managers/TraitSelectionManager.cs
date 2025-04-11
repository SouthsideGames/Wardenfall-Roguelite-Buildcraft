using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TraitSelectionManager : MonoBehaviour
{
    public static TraitSelectionManager Instance;

    [Header("UI References")]
    [SerializeField] private GameObject panel;
    [SerializeField] private TraitOptionUI[] traitOptions; // 3 buttons or cards

    [Header("Trait Settings")]
    [SerializeField] private List<EnemyTraitDataSO> allTraits; // ScriptableObject refs
    [SerializeField] private int optionsToShow = 3;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void OpenTraitSelection()
    {
       panel.SetActive(true);

        // Shuffle available traits to pick from
        List<EnemyTraitDataSO> available = new List<EnemyTraitDataSO>(allTraits);
        int displayCount = Mathf.Min(optionsToShow, traitOptions.Length, available.Count);

        for (int i = 0; i < displayCount; i++)
        {
            int index = Random.Range(0, available.Count);
            EnemyTraitDataSO selected = available[index];
            available.RemoveAt(index);

            int stacks = EnemyTraitManager.Instance.GetStackCount(selected.TraitID);
            EnemyTraitTier tier = selected.GetTier(stacks + 1); // Preview next tier

            traitOptions[i].gameObject.SetActive(true);
            traitOptions[i].Configure(
                selected.TraitID,
                tier.TierName,
                tier.Description,
                () => OnTraitSelected(selected.TraitID)
            );
        }

        // Disable unused options if any
        for (int i = displayCount; i < traitOptions.Length; i++)
            traitOptions[i].gameObject.SetActive(false);
    }

    private void OnTraitSelected(string traitID)
    {
        panel.SetActive(false);
        EnemyTraitManager.Instance.AddTrait(traitID);
        GameManager.Instance.WaveCompletedCallback(); // Continue game
    }
}
