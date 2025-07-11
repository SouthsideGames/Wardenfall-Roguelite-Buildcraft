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

        List<TraitDataSO> available = new List<TraitDataSO>(TraitManager.Instance.AllTraits);
        int displayCount = Mathf.Min(optionsToShow, available.Count);

        for (int i = 0; i < displayCount; i++)
        {
            int index = Random.Range(0, available.Count);
            TraitDataSO selected = available[index];
            available.RemoveAt(index);

            int stacks = TraitManager.Instance.GetStackCount(selected.TraitID);
            TraitTier tier = selected.GetTier(stacks + 1);

            TraitOptionUI card = Instantiate(traitOptionPrefab, traitCardContainer);
            card.Configure(selected.TraitID, tier.TierName, tier.Description, () => OnTraitSelected(selected.TraitID));

        }

        
    }
    
    public void CloseTraitSelection() => panel.SetActive(false);

    public void GameStateChangedCallback(GameState state)
    {
        if (state == GameState.TraitSelection)
        {
            if (ChallengeManager.IsActive(ChallengeMode.TraitChaos))
            {
                TraitManager.Instance.AddRandomTrait(); 
                GameManager.Instance.StartCardDraft(); 
            }
            else
            {
                OpenTraitSelection(); // Default behavior
            }
        }
        else
        {
            CloseTraitSelection();
        }
    }


    private void OnTraitSelected(string traitID)
    {
        TraitManager.Instance.AddTrait(traitID);
        GameManager.Instance.StartCardDraft();
    }
}
