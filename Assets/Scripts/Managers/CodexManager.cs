using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using SouthsideGames.SaveManager;
using Unity.VisualScripting;

public class CodexManager : MonoBehaviour
{
    [Header("ELEMENTS:")]
    [SerializeField] private TMP_Dropdown categoryDropdown;
    [SerializeField] private Transform detailParent;
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private TextMeshProUGUI progressText;
    [SerializeField] private CardLibrary cardLibrary;

    [Header("DETAIL VIEW:")]
    [SerializeField] private GameObject detailContainer;
    [SerializeField] private Image detailIcon;
    [SerializeField] private TextMeshProUGUI detailName;
    [SerializeField] private TextMeshProUGUI detailDescription;
    [SerializeField] private Transform statContainersParent;

    [Header("ENEMY VIEW:")]
    [SerializeField] private GameObject enemyDetailContainer;
    [SerializeField] private Image enemyDetailIcon;
    [SerializeField] private TextMeshProUGUI enemyDetailName;
    [SerializeField] private TextMeshProUGUI enemyDetailDescription;
    [SerializeField] private TextMeshProUGUI enemyDetailTypeText;

    [Header("OBJECT VIEW:")]
    [SerializeField] private GameObject objectDetailContainer;
    [SerializeField] private Image objectDetailIcon;
    [SerializeField] private TextMeshProUGUI objectDetailName;
    [SerializeField] private Transform objectStatContainersParent;

    [Header("CARD VIEW:")]
    [SerializeField] private Image cardDetailIcon;
    [SerializeField] private TextMeshProUGUI cardDetailName;
    [SerializeField] private GameObject cardDetailContainer;
    [SerializeField] private TextMeshProUGUI cardDetailCost;
    [SerializeField] private TextMeshProUGUI cardId;
    [SerializeField] private TextMeshProUGUI cardDetailDescription;
    [SerializeField] private TextMeshProUGUI cardDetailRarity;
    [SerializeField] private TextMeshProUGUI cardDetailCooldown;
    [SerializeField] private TextMeshProUGUI cardDetailDuration;
    [SerializeField] private TextMeshProUGUI cardDetailSynergies;
    [SerializeField] private CardSynergyManager synergyManager;
    [SerializeField] private Button unlockButton;


    [SerializeField] private GameObject statPrefab;

    private void Awake() => InitializeDropdown();

    private void Start()
    {
        CloseDetailView();
        LoadAndDisplayCharacterCards();
        CardUnlockManager.Instance.LoadAllCardUnlockStates(cardLibrary);
    }

    private void InitializeDropdown()
    {
        categoryDropdown.ClearOptions();
        var categories = new List<string> { "Characters", "Weapons", "Objects", "Enemies", "Cards" };
        categoryDropdown.AddOptions(categories);
        categoryDropdown.onValueChanged.AddListener(OnDropdownSelectionChanged);
    }

    private void OnDropdownSelectionChanged(int selectedIndex)
    {
        switch (selectedIndex)
        {
            case 0:
                LoadAndDisplayCharacterCards();
                break;
            case 1:
                LoadAndDisplayWeaponCards();
                break;
            case 2:
                LoadAndDisplayObjectCards();
                break;
            case 3:
                LoadAndDisplayEnemyCards();
                break;
            case 4:
                LoadAndDisplayCardCodex();
                break;
            default:
                Debug.LogWarning("Invalid category selected.");
                break;
        }
    }

    #region Card Codex View

    private void LoadAndDisplayCardCodex()
    {
        ClearCards();
        progressText.gameObject.SetActive(true);
        var allCards = cardLibrary.allCards;

        foreach (var card in allCards)
        {
            GameObject miniCard = Instantiate(cardPrefab, detailParent);
            CodexCardUI cardUI = miniCard.GetComponent<CodexCardUI>();
            cardUI.InitializeCardCodex(card, this);
        }

        int unlockedCount = allCards.FindAll(c => c.isUnlocked).Count;
        int total = allCards.Count;
        progressText.text = $"Unlocked: {unlockedCount} / {total} cards ({(int)((float)unlockedCount / total * 100)}%)";
    }

    public void OpenCardDetail(CardSO card)
    {
        if (card == null) return;

        if (card.isUnlocked)
        {
            unlockButton.gameObject.SetActive(false);
        }
        else
        {
            bool canUnlock = CardUnlockManager.Instance.CanUnlock(card);
            unlockButton.gameObject.SetActive(true);
            unlockButton.interactable = canUnlock;

            unlockButton.onClick.RemoveAllListeners();
            unlockButton.onClick.AddListener(() =>
            {
                if (CardUnlockManager.Instance.TryUnlockCard(card))
                {
                    LoadAndDisplayCardCodex();
                    OpenCardDetail(card); 
                }
            });
        }

        if (cardId != null) cardId.text = $"ID: {card.cardID}";
        if (cardDetailIcon != null) cardDetailIcon.sprite = card.icon;
        if (cardDetailName != null) cardDetailName.text = card.cardName;
        if (cardDetailDescription != null) cardDetailDescription.text = card.isUnlocked ? card.description : $"Required Card Points: {card.requiredCardPoints}";

        if (cardDetailCost != null) cardDetailCost.text = $"Cost: {card.cost}";
        if (cardDetailRarity != null) cardDetailRarity.text = $"Rarity: {card.rarity}";
        if (cardDetailCooldown != null) cardDetailCooldown.text = card.cooldown > 0 ? $"Cooldown: {card.cooldown}s" : "";
        if (cardDetailDuration != null) cardDetailDuration.text = card.activeTime > 0 ? $"Duration: {card.activeTime}s" : "";

        if (cardDetailSynergies != null && synergyManager != null)
        {
            var cardSynergies = synergyManager.GetSynergiesForCard(card.effectType);
            if (cardSynergies.Count > 0)
            {
                System.Text.StringBuilder sb = new System.Text.StringBuilder("Synergies:\n");
                foreach (var (partnerType, resultCard) in cardSynergies)
                {
                    sb.AppendLine($"+ {partnerType} → {resultCard.cardName}");
                }
                cardDetailSynergies.text = sb.ToString();
            }
            else
            {
                cardDetailSynergies.text = "";
            }
        }

        if (cardDetailContainer != null) cardDetailContainer.SetActive(true);
    }

    #endregion

    #region Card Loading Methods

    private void LoadAndDisplayCharacterCards()
    {
        ClearCards();
        progressText.gameObject.SetActive(false);
        var characterDataItems = Resources.LoadAll<CharacterDataSO>("Data/Characters");

        foreach (var characterData in characterDataItems)
        {
            GameObject miniCard = Instantiate(cardPrefab, detailParent);
            CodexCardUI cardUI = miniCard.GetComponent<CodexCardUI>();
            cardUI.InitializeCharacterCard(characterData.Icon, characterData.Name, characterData, this);
        }
    }

    private void LoadAndDisplayWeaponCards()
    {
        ClearCards();
        progressText.gameObject.SetActive(false);
        var weaponItems = Resources.LoadAll<WeaponDataSO>("Data/Weapons");

        foreach (var weaponData in weaponItems)
        {
            GameObject miniCard = Instantiate(cardPrefab, detailParent);
            CodexCardUI cardUI = miniCard.GetComponent<CodexCardUI>();
            cardUI.InitializeWeaponCard(weaponData.Icon, weaponData.Name, weaponData, this);
        }
    }

    private void LoadAndDisplayObjectCards()
    {
        ClearCards();
        progressText.gameObject.SetActive(false);
        var objectItems = Resources.LoadAll<ObjectDataSO>("Data/Objects");

        foreach (var objectData in objectItems)
        {
            GameObject miniCard = Instantiate(cardPrefab, detailParent);
            CodexCardUI cardUI = miniCard.GetComponent<CodexCardUI>();
            cardUI.InitializeObjectCard(objectData.Icon, objectData.Name, objectData, this);
        }
    }

    private void LoadAndDisplayEnemyCards()
    {
        ClearCards();
        progressText.gameObject.SetActive(false);
        var enemyItems = Resources.LoadAll<EnemyDataSO>("Data/Enemies");

        foreach (var enemyData in enemyItems)
        {
            GameObject miniCard = Instantiate(cardPrefab, detailParent);
            CodexCardUI cardUI = miniCard.GetComponent<CodexCardUI>();
            cardUI.InitializeEnemyCard(enemyData.Icon, enemyData.Name, enemyData, this);
        }
    }

    #endregion

    #region Detail View Methods

    public void OpenDetailView(CharacterDataSO _characterData)
    {
        detailIcon.sprite = _characterData.Icon;
        detailName.text = _characterData.Name;
        detailDescription.text = _characterData.Description;
        DisplayCharacterStats(_characterData);
        detailContainer.SetActive(true);
    }

    public void OpenWeaponDetailView(WeaponDataSO _weaponData)
    {
        detailIcon.sprite = _weaponData.Icon;
        detailName.text = _weaponData.Name;
        detailDescription.text = _weaponData.Description;
        statContainersParent.Clear();
        DisplayWeaponStats(_weaponData);
        detailContainer.SetActive(true);
    }

    public void OpenObjectDetailView(ObjectDataSO _objectData)
    {
        objectDetailIcon.sprite = _objectData.Icon;
        objectDetailName.text = _objectData.Name;
        statContainersParent.Clear();
        DisplayObjectStats(_objectData);
        objectDetailContainer.SetActive(true);
    }

    public void OpenEnemyDetailView(EnemyDataSO enemyData)
    {
        enemyDetailIcon.sprite = enemyData.Icon;
        enemyDetailName.text = enemyData.Name;
        enemyDetailDescription.text = enemyData.Description;
        enemyDetailTypeText.text = $"Type: {enemyData.Type}";

        enemyDetailContainer.SetActive(true);
    }

    private void DisplayCharacterStats(CharacterDataSO characterData)
    {
        statContainersParent.Clear();

        foreach (var stat in characterData.NonNeutralStats)
        {
            GameObject statEntry = Instantiate(statPrefab, statContainersParent);
            StatContainerUI statContainerUI = statEntry.GetComponent<StatContainerUI>();
            Sprite statIcon = ResourceManager.GetStatIcon(stat.Key);
            statContainerUI.Configure(statIcon, Enums.FormatStatName(stat.Key), stat.Value, true);
        }
    }

    private void DisplayWeaponStats(WeaponDataSO weaponData)
    {
        statContainersParent.Clear();

        foreach (var stat in weaponData.BaseStats)
        {
            GameObject statEntry = Instantiate(statPrefab, statContainersParent);
            StatContainerUI statContainerUI = statEntry.GetComponent<StatContainerUI>();
            Sprite statIcon = ResourceManager.GetStatIcon(stat.Key);
            statContainerUI.Configure(statIcon, Enums.FormatStatName(stat.Key), stat.Value, true);
        }
    }

    private void DisplayObjectStats(ObjectDataSO _objectData)
    {
        objectStatContainersParent.Clear();

        foreach (var stat in _objectData.BaseStats)
        {
            GameObject statEntry = Instantiate(statPrefab, objectStatContainersParent);
            StatContainerUI statContainerUI = statEntry.GetComponent<StatContainerUI>();
            Sprite statIcon = ResourceManager.GetStatIcon(stat.Key);
            statContainerUI.Configure(statIcon, Enums.FormatStatName(stat.Key), stat.Value, false);
        }
    }

    public void CloseDetailView() => detailContainer.SetActive(false);
    public void CloseEnemyDetailView() => enemyDetailContainer.SetActive(false);
    public void CloseObjectDetailView() => objectDetailContainer.SetActive(false);
    public void CloseCardDetailView() => cardDetailContainer.SetActive(false);
    private void ClearCards() => detailParent.Clear();

    #endregion

}