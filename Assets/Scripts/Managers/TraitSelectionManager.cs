using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TraitSelectionManager : MonoBehaviour, IGameStateListener
{
    public static TraitSelectionManager Instance;

    [Header("UI References")]
    [SerializeField] private GameObject panel;
    [SerializeField] private Transform traitCardContainer;
    [SerializeField] private TraitOptionUI traitOptionPrefab;

    [Header("Trait Settings")]
    [SerializeField] private List<EnemyTraitDataSO> allTraits;
    [SerializeField] private int optionsToShow = 3;

    private void Awake()
    {
        if(Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }


    public void OpenTraitSelection()
    {
        panel.SetActive(true);

        foreach (Transform child in traitCardContainer)
            Destroy(child.gameObject);

        List<EnemyTraitDataSO> available = new List<EnemyTraitDataSO>(allTraits);
        int displayCount = Mathf.Min(optionsToShow, available.Count);

        for (int i = 0; i < displayCount; i++)
        {
            int index = Random.Range(0, available.Count);
            EnemyTraitDataSO selected = available[index];
            available.RemoveAt(index);

            int stacks = EnemyTraitManager.Instance.GetStackCount(selected.TraitID);
            EnemyTraitTier tier = selected.GetTier(stacks + 1);

            TraitOptionUI card = Instantiate(traitOptionPrefab, traitCardContainer);
            card.Configure(selected.TraitID, tier.TierName, tier.Description, () => OnTraitSelected(selected.TraitID));

        }
    }
    
    public void CloseTraitSelection() => panel.SetActive(false);

    public void GameStateChangedCallback(GameState state)
    {
        if (state == GameState.TraitSelection)
            OpenTraitSelection();
        else
            CloseTraitSelection();
    }

    private void OnTraitSelected(string traitID)
    {
        EnemyTraitManager.Instance.AddTrait(traitID);
        GameManager.Instance.StartShop(); // Proceed to shop
    }
}
