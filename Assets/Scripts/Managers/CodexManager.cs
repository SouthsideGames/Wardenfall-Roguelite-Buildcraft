using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class CodexManager : MonoBehaviour
{
    [Header("ELEMENTS:")]
    [SerializeField] private TMP_Dropdown categoryDropdown;
    [SerializeField] private Transform detailParent;
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private TextMeshProUGUI progressText;
    [SerializeField] private CardLibrary cardLibrary;
    [SerializeField] private GameObject detailContainer;

    [Header("DETAIL VIEW:")]
    [SerializeField] private Image detailIcon;
    [SerializeField] private TextMeshProUGUI detailName;
    [SerializeField] private TextMeshProUGUI detailDescription;
    [SerializeField] private Transform statContainersParent;

    [Header("ENEMY VIEW:")]
    [SerializeField] private GameObject enemyDetailContainer;
    [SerializeField] private Image enemyDetailIcon;
    [SerializeField] private TextMeshProUGUI enemyDetailName;
    [SerializeField] private TextMeshProUGUI enemyDetailDescription;

    [Header("OBJECT VIEW:")]
    [SerializeField] private GameObject objectDetailContainer;
    [SerializeField] private Image objectDetailIcon;
    [SerializeField] private TextMeshProUGUI objectDetailName;
    [SerializeField] private Transform objectStatContainersParent;

    [Header("CARD VIEW:")]
    [SerializeField] private GameObject cardDetailContainer;
    [SerializeField] private TextMeshProUGUI detailCost;
    [SerializeField] private TextMeshProUGUI detailType;
    [SerializeField] private TextMeshProUGUI detailRarity;
    [SerializeField] private TextMeshProUGUI detailCooldown; // optional
    [SerializeField] private TextMeshProUGUI detailDuration; // optional

    [SerializeField] private GameObject statPrefab;
    [SerializeField] private Sprite lockedIcon;

    private void Awake() => InitializeDropdown();

    private void Start()
    {
        CloseDetailView();
        LoadAndDisplayCharacterCards();
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

    public void OpenEnemyDetailView(EnemyDataSO _enemyData)
    {
        enemyDetailIcon.sprite = _enemyData.Icon;
        enemyDetailName.text = _enemyData.Name;
        enemyDetailDescription.text = _enemyData.Description;
        enemyDetailContainer.SetActive(true);
    }

    public void OpenCardDetail(CardSO card)
    {
        detailIcon.sprite = card.isUnlocked ? card.icon : lockedIcon;
        detailName.text = card.isUnlocked ? card.cardName : "???";
        detailDescription.text = card.isUnlocked ? card.description : card.unlockHint;

        if (card.isUnlocked)
        {
            detailCost.text = $"Cost: {card.cost}";
            detailType.text = $"Type: {card.effectType}";
            detailRarity.text = $"Rarity: {card.rarity}";

            if (detailCooldown != null)
                detailCooldown.text = card.cooldown > 0 ? $"Cooldown: {card.cooldown}s" : "";

            if (detailDuration != null)
                detailDuration.text = card.activeTime > 0 ? $"Duration: {card.activeTime}s" : "";
        }
        else
        {
            detailCost.text = "";
            detailType.text = "";
            detailRarity.text = "";
            if (detailCooldown != null) detailCooldown.text = "";
            if (detailDuration != null) detailDuration.text = "";
        }

        cardDetailContainer.SetActive(true);
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
            statContainerUI.Configure(statIcon, Enums.FormatStatName(stat.Key), stat.Value, true);
        }
    }

    public void CloseDetailView() => detailContainer.SetActive(false);
    public void CloseEnemyDetailView() => enemyDetailContainer.SetActive(false);
    public void CloseObjectDetailView() => objectDetailContainer.SetActive(false);
    public void CloseCardDetailView() => cardDetailContainer.SetActive(false);

    private void ClearCards() => detailParent.Clear();

    #endregion
}
