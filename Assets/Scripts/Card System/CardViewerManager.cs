using UnityEngine;
using TMPro;

public class CardViewerManager : MonoBehaviour
{
    [SerializeField] private Transform currentCardContainer;  
    [SerializeField] private TextMeshProUGUI currentCostText;

    [Header("VIEW CARD RARITY PREFABS")]
    [SerializeField] private CardViewOptionUI commonOptionPrefab;
    [SerializeField] private CardViewOptionUI uncommonOptionPrefab;
    [SerializeField] private CardViewOptionUI rareOptionPrefab;
    [SerializeField] private CardViewOptionUI epicOptionPrefab;
    [SerializeField] private CardViewOptionUI legendaryOptionPrefab;
    [SerializeField] private CardViewOptionUI mythicOptionPrefab;
    [SerializeField] private CardViewOptionUI exaltedOptionPrefab;

    [Header("VIEW CARD PANEL")]
    [SerializeField] private GameObject viewCardPanel;

    void Start()
    {
        HideViewCardPanel();
    }


    public void InitializeCards()
    {
        currentCostText.text = $"{CharacterManager.Instance.cards.currentTotalCost} / {CharacterManager.Instance.cards.GetEffectiveDeckCap()}";

        foreach (Transform child in currentCardContainer)
        {
            Destroy(child.gameObject);
        }

        foreach (CardSO card in CharacterManager.Instance.cards.Deck)
        {
            CardViewOptionUI prefabToUse = GetPrefabByRarity(card.rarity);
            if (prefabToUse == null) continue;

            CardViewOptionUI newCardUI = Instantiate(prefabToUse, currentCardContainer);
            newCardUI.Configure(card);
        }
    }

    private CardViewOptionUI GetPrefabByRarity(CardRarity rarity)
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
            _ => null
        };
    }

    public void ShowViewCardPanel()
    {
        viewCardPanel.SetActive(true);
        InitializeCards();
    }

    public void HideViewCardPanel()
    {
        viewCardPanel.SetActive(false);
        InitializeCards();
    }

}
