using UnityEngine;
using TMPro;

public class ViewCardManager : MonoBehaviour
{
    [SerializeField] private Transform currentCardContainer;  
    [SerializeField] private TextMeshProUGUI currentCostText;

    [Header("VIEW CARD RARITY PREFABS")]
    [SerializeField] private ViewCardOptionUI commonOptionPrefab;
    [SerializeField] private ViewCardOptionUI uncommonOptionPrefab;
    [SerializeField] private ViewCardOptionUI rareOptionPrefab;
    [SerializeField] private ViewCardOptionUI epicOptionPrefab;
    [SerializeField] private ViewCardOptionUI legendaryOptionPrefab;
    [SerializeField] private ViewCardOptionUI mythicOptionPrefab;
    [SerializeField] private ViewCardOptionUI exaltedOptionPrefab;


    public void InitializeCards()
    {
        // Update cost display
        currentCostText.text = $"{CharacterManager.Instance.cards.currentTotalCost} / {CharacterManager.Instance.cards.GetEffectiveDeckCap()}";

        // Clear old cards
        foreach (Transform child in currentCardContainer)
        {
            Destroy(child.gameObject);
        }

        // Create cards based on rarity
        foreach (CardSO card in CharacterManager.Instance.cards.Deck)
        {
            ViewCardOptionUI prefabToUse = GetPrefabByRarity(card.rarity);
            if (prefabToUse == null) continue;

            ViewCardOptionUI newCardUI = Instantiate(prefabToUse, currentCardContainer);
            newCardUI.Configure(card);
        }
    }

    private ViewCardOptionUI GetPrefabByRarity(CardRarity rarity)
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
}
