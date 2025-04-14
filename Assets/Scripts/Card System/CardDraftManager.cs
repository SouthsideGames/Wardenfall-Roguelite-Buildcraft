using System.Collections.Generic;
using UnityEngine;

public class CardDraftManager : MonoBehaviour, IGameStateListener
{
    [SerializeField] private GameObject panel;
    [SerializeField] private Transform cardContainer;
    [SerializeField] private CardLibrary cardLibrary;
    [SerializeField] private int optionsToShow = 3;

    [Header("RARITY PREFABS")]
    [SerializeField] private CardOptionUI commonOptionPrefab;
    
    [SerializeField] private CardOptionUI uncommonOptionPrefab;
    [SerializeField] private CardOptionUI rareOptionPrefab;
    [SerializeField] private CardOptionUI epicOptionPrefab;
    [SerializeField] private CardOptionUI legendaryOptionPrefab;
    [SerializeField] private CardOptionUI mythicOptionPrefab;
    [SerializeField] private CardOptionUI exaltedOptionPrefab;

    public void GameStateChangedCallback(GameState state)
    {
        if (state == GameState.CardDraft)
            OpenCardDraft();
        else
            CloseCardDraft();
    }

    private void OpenCardDraft()
    {
        panel.SetActive(true);

        foreach (Transform child in cardContainer)
            Destroy(child.gameObject);

        List<CardSO> available = new List<CardSO>(cardLibrary.allCards);
        int displayCount = Mathf.Min(optionsToShow, available.Count);

        for (int i = 0; i < displayCount; i++)
        {
            int index = Random.Range(0, available.Count);
            CardSO selected = available[index];
            available.RemoveAt(index);

            CardOptionUI card = Instantiate(GetOptionPrefab(selected.rarity), cardContainer);
            card.Configure(selected, () => OnCardSelected(selected));
        }
    }

    private CardOptionUI GetOptionPrefab(CardRarity rarity)
    {
        return rarity switch
        {
            CardRarity.Common => commonOptionPrefab,
            CardRarity.Uncommon => uncommonOptionPrefab,
            CardRarity.Rare => rareOptionPrefab,
            CardRarity.Epic => epicOptionPrefab,
            CardRarity.Legendary => legendaryOptionPrefab,
            CardRarity.Mythic => mythicOptionPrefab,
            CardRarity.Exalted => exaltedOptionPrefab,
            _ => commonOptionPrefab
        };
    }

    private void CloseCardDraft() => panel.SetActive(false);

    private void OnCardSelected(CardSO card)
    {
        CharacterManager.Instance.cards.AddCard(card);
        GameManager.Instance.StartTraitSelection();
    }
} 
