using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

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
    [SerializeField] private RectTransform detailContainer;
    [SerializeField] private Image detailIcon;
    [SerializeField] private TextMeshProUGUI cardIdText;
    [SerializeField] private TextMeshProUGUI detailTypeText;
    [SerializeField] private TextMeshProUGUI detailName;
    [SerializeField] private TextMeshProUGUI detailDescription;
    [SerializeField] private TextMeshProUGUI detailCost;
    [SerializeField] private TextMeshProUGUI detailRarity;
    [SerializeField] private TextMeshProUGUI synergyText;
    [SerializeField] private GameObject statsHolder;
    [SerializeField] private TextMeshProUGUI unlockText;
    [SerializeField] private Button unlockButton;

    [Header("PROGRESSION")]
    [SerializeField] private TextMeshProUGUI progressionText;

    private List<CardSO> filteredCards = new();

    private void Start()
    {
        damageButton.onClick.AddListener(() => LoadCardsByType(CardType.Damage));
        supportButton.onClick.AddListener(() => LoadCardsByType(CardType.Support));
        utilityButton.onClick.AddListener(() => LoadCardsByType(CardType.Utility));

        detailContainer.gameObject.SetActive(false);
        progressionText.gameObject.SetActive(false);

        damageButton.onClick.Invoke();
    }

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
        detailContainer.gameObject.SetActive(false);
    }

    public void ShowCardDetail(CardSO card)
    {
        if (card == null) return;

        detailIcon.sprite = card.icon;
        detailName.text = card.cardName;
        cardIdText.text = $"ID: {card.cardID}";
        detailTypeText.text = $"Type: {card.cardType}";
        detailCost.text = $"Cost: {card.cost}";
        detailRarity.text = $"Rarity: {card.rarity}";

        bool isUnlocked = card.unlockData != null && card.unlockData.unlocked;

        if (isUnlocked)
        {
            statsHolder.SetActive(true);
            unlockText.gameObject.SetActive(false);
            detailDescription.text = card.description;

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
        }
        else
        {
            statsHolder.SetActive(false);
            unlockText.gameObject.SetActive(true);
            detailDescription.text = "";
            synergyText.text = "";
            unlockText.text = $"Requires {card.unlockData.unlockCost} tickets to unlock.";
        }

        if (isUnlocked)
        {
            unlockButton.gameObject.SetActive(false);
        }
        else
        {
            unlockButton.gameObject.SetActive(true);
            unlockButton.onClick.RemoveAllListeners();
            unlockButton.onClick.AddListener(() => AttemptUnlock(card));
        }

        ShowDetailPanel();
    }

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

    private void ClearCardList()
    {
        foreach (Transform child in cardListParent)
            Destroy(child.gameObject);
    }

    private void AttemptUnlock(CardSO card)
    {
        if (CardUnlockManager.Instance.TryUnlockCard(card))
        {
            ShowCardDetail(card); // Refresh UI
            UpdateProgressionText(); // Update progression %
        }
        else
        {
            Debug.Log("Unlock failed. Not enough tickets?");
        }
    }

    public void ShowDetailPanel() => detailContainer.gameObject.SetActive(true);

    public void CloseDetailPanel() => detailContainer.gameObject.SetActive(false);
}
