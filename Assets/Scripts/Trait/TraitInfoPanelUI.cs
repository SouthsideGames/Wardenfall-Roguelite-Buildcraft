using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class TraitInfoPanelUI : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    [SerializeField] private Transform traitListContainer;
    [SerializeField] private TraitEntryUI traitEntryPrefab;

    private void OnEnable()
    {
        GameManager.OnGamePaused += ShowActiveTraits;
        GameManager.OnGameResumed += Hide;
    }

    private void OnDisable()
    {
        GameManager.OnGamePaused -= ShowActiveTraits;
        GameManager.OnGameResumed -= Hide;
    }

    private void ShowActiveTraits()
    {
        panel.SetActive(true);

        foreach (Transform child in traitListContainer)
            Destroy(child.gameObject);

        List<(TraitDataSO trait, int stack)> activeTraits = TraitManager.Instance.GetAllActiveTraits();

        foreach (var (trait, stack) in activeTraits)
        {
            if (trait == null) continue;

            var entry = Instantiate(traitEntryPrefab, traitListContainer);
            entry.Setup(trait, stack);
        }
    }

    private void Hide()
    {
        panel.SetActive(false);
    }
}
