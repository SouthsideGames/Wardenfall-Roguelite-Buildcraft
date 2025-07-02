using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GearShopManager : MonoBehaviour
{
    [Header("DATA")]
    [SerializeField] private CardLibrary cardLibrary;
    [SerializeField] private CardSynergyManager synergyManager;

    [Header("UI BUTTONS")]
    [SerializeField] private Button damageButton;
    [SerializeField] private Button supportButton;
    [SerializeField] private Button utilityButton;

    [Header("CARD LIST")]
    [SerializeField] private Transform cardListParent;
    [SerializeField] private GameObject cardListItemPrefab;

    [Header("DETAIL VIEW")]
    [SerializeField] private GameObject detailContainer;
    [SerializeField] private Image detailIcon;
    [SerializeField] private TextMeshProUGUI detailName;
    [SerializeField] private TextMeshProUGUI detailDescription;
    [SerializeField] private TextMeshProUGUI detailCost;
    [SerializeField] private TextMeshProUGUI detailRarity;
    [SerializeField] private TextMeshProUGUI synergyText;

    [Header("PROGRESSION")]
    [SerializeField] private TextMeshProUGUI progressionText;

    private List<CardSO> filteredCards = new();

    private void Start()
    {
        // Set up button listeners
        damageButton.onClick.AddListener(() => LoadCardsByType(CardType.Damage));
        supportButton.onClick.AddListener(() => LoadCardsByType(CardType.Support));
        utilityButton.onClick.AddListener(() => LoadCardsByType(CardType.Utility));

        detailContainer.SetActive(false);
        progressionText.gameObject.SetActive(false);
    }

    /// <summary>
    /// Loads and displays all cards of the specified type.
    /// </summary>
    private void LoadCardsByType(CardType type)
    {
        ClearCardList();

        filteredCards = cardLibrary.allCards
            .Where(c => c.cardType == type)
            .ToList();

        foreach (var card in filteredCards)
        {
            var item = Instantiate(cardListItemPrefab, cardListParent);
            var ui = item.GetComponent<GearRoomCardUI>();
            ui.Initialize(card, this);
        }

        UpdateProgressionText();
        detailContainer.SetActive(false);
    }

    /// <summary>
    /// Shows the detailed information for the selected card.
    /// </summary>
    public void ShowCardDetail(CardSO card)
    {
        if (card == null) return;

        // Icon and name
        detailIcon.sprite = card.icon;
        detailName.text = card.cardName;

        // Description (locked/unlocked)
        detailDescription.text = card.unlockData != null && card.unlockData.unlocked
            ? card.description
            : $"Required Unlock Tickets: {card.unlockData.unlockCost}";

        // Cost and rarity
        detailCost.text = $"Cost: {card.cost}";
        detailRarity.text = $"Rarity: {card.rarity}";

        // Synergy
        if (synergyManager != null && synergyText != null)
        {
            var synergies = synergyManager.GetSynergiesForCard(card.effectType);
            if (synergies.Count > 0)
            {
                System.Text.StringBuilder sb = new System.Text.StringBuilder("Synergies:\n");
                foreach (var (partnerType, resultCard) in synergies)
                {
                    sb.AppendLine($"+ {partnerType} → {resultCard.cardName}");
                }
                synergyText.text = sb.ToString();
            }
            else
            {
                synergyText.text = "";
            }
        }

        detailContainer.SetActive(true);
    }

    /// <summary>
    /// Updates the progression text to show unlock count and percentage.
    /// </summary>
    private void UpdateProgressionText()
    {
        if (progressionText == null) return;

        int unlocked = filteredCards.Count(c => c.unlockData != null && c.unlockData.unlocked);
        int total = filteredCards.Count;

        if (total > 0)
        {
            float percent = (float)unlocked / total * 100f;
            progressionText.text = $"Unlocked: {unlocked} / {total} cards ({(int)percent}%)";
            progressionText.gameObject.SetActive(true);
        }
        else
        {
            progressionText.text = "No cards in this category.";
            progressionText.gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// Clears all spawned card UI elements in the list.
    /// </summary>
    private void ClearCardList()
    {
        foreach (Transform child in cardListParent)
        {
            Destroy(child.gameObject);
        }
    }
}
