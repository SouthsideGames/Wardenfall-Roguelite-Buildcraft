using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using SouthsideGames.SaveManager;
using Unity.VisualScripting;
using System.Linq;

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
    [SerializeField] private Image abilityIcon;
    [SerializeField] private TextMeshProUGUI abilityName;
    [SerializeField] private TextMeshProUGUI abilityDescription;
    [SerializeField] private Transform statContainersParent;

    [Header("ENEMY VIEW:")]
    [SerializeField] private GameObject enemyDetailContainer;
    [SerializeField] private Image enemyDetailIcon;
    [SerializeField] private TextMeshProUGUI enemyDetailName;
    [SerializeField] private TextMeshProUGUI enemyDetailDescription;
    [SerializeField] private TextMeshProUGUI enemyDetailTypeText;
    [SerializeField] private GameObject evolutionHolder;
    [SerializeField] private Image evolutionPreviewImage;
    [SerializeField] private Button evolutionButton;

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

    private List<EnemyDataSO> loadedEnemies = new();
    private int currentEnemyIndex = 0;
    private List<CharacterDataSO> loadedCharacters = new();
    private int currentCharacterIndex = 0;
    private List<WeaponDataSO> loadedWeapons = new();
    private int currentWeaponIndex = 0;
    private List<ObjectDataSO> loadedObjects = new();
    private int currentObjectIndex = 0;
    private List<CardSO> loadedCards = new();
    private int currentCardIndex = 0;

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
            case 0: LoadAndDisplayCharacterCards(); break;
            case 1: LoadAndDisplayWeaponCards(); break;
            case 2: LoadAndDisplayObjectCards(); break;
            case 3: LoadAndDisplayEnemyCards(); break;
            case 4: LoadAndDisplayCardCodex(); break;
            default: Debug.LogWarning("Invalid category selected."); break;
        }
    }

    private void LoadAndDisplayCardCodex()
    {
        ClearCards();
        progressText.gameObject.SetActive(true);
        var allCards = cardLibrary.allCards;

        loadedCards = cardLibrary.allCards.ToList();
        currentCardIndex = 0;

        foreach (var card in allCards)
        {
            GameObject miniCard = Instantiate(cardPrefab, detailParent);
            CodexCardUI cardUI = miniCard.GetComponent<CodexCardUI>();
            cardUI.InitializeCardCodex(card, this);
        }

        int unlockedCount = allCards.FindAll(c => c.unlockData != null && c.unlockData.unlocked).Count;
        int total = allCards.Count;
        progressText.text = $"Unlocked: {unlockedCount} / {total} cards ({(int)((float)unlockedCount / total * 100)}%)";
    }

    public void OpenCardDetail(CardSO card)
    {
        if (card == null) return;

        if (card.unlockData != null && card.unlockData.unlocked)
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

        cardDetailDescription.text = card.unlockData != null && card.unlockData.unlocked
            ? card.description
            : $"Required Unlock Tickets: {card.unlockData.unlockCost}";

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

        currentCardIndex = loadedCards.IndexOf(card);

        if (cardDetailContainer != null) cardDetailContainer.SetActive(true);
    }

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
        loadedWeapons = new List<WeaponDataSO>(Resources.LoadAll<WeaponDataSO>("Data/Weapons"));
        currentWeaponIndex = 0;
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

        loadedObjects = new List<ObjectDataSO>(Resources.LoadAll<ObjectDataSO>("Data/Objects"));
        currentObjectIndex = 0;

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
        loadedEnemies = new List<EnemyDataSO>(Resources.LoadAll<EnemyDataSO>("Data/Enemies"));
        currentEnemyIndex = 0;

        foreach (var enemyData in loadedEnemies)
        {
            GameObject miniCard = Instantiate(cardPrefab, detailParent);
            CodexCardUI cardUI = miniCard.GetComponent<CodexCardUI>();
            cardUI.InitializeEnemyCard(enemyData.Icon, enemyData.Name, enemyData, this);
        }
    }


    public void OpenDetailView(CharacterDataSO _characterData)
    {
        detailIcon.sprite = _characterData.Icon;
        detailName.text = _characterData.Name;
        detailDescription.text = _characterData.Description;
        abilityIcon.sprite = _characterData.AbilityIcon;
        abilityName.text = _characterData.AbilityName;
        abilityDescription.text = _characterData.AbilityDescription;
        DisplayCharacterStats(_characterData);
        detailContainer.SetActive(true);
        currentCharacterIndex = loadedCharacters.IndexOf(_characterData);
    }

    public void OpenWeaponDetailView(WeaponDataSO _weaponData)
    {
        detailIcon.sprite = _weaponData.Icon;
        detailName.text = _weaponData.Name;
        detailDescription.text = _weaponData.Description;
        statContainersParent.Clear();
        DisplayWeaponStats(_weaponData);
        detailContainer.SetActive(true);
        currentWeaponIndex = loadedWeapons.IndexOf(_weaponData);
    }

    public void OpenObjectDetailView(ObjectDataSO _objectData)
    {
        objectDetailIcon.sprite = _objectData.Icon;
        objectDetailName.text = _objectData.Name;
        statContainersParent.Clear();
        DisplayObjectStats(_objectData);
        objectDetailContainer.SetActive(true);
        currentObjectIndex = loadedObjects.IndexOf(_objectData);
    }

    public void OpenEnemyDetailView(EnemyDataSO enemyData)
    {
        currentEnemyIndex = loadedEnemies.IndexOf(enemyData);

        enemyDetailIcon.sprite = enemyData.Icon;
        enemyDetailName.text = enemyData.Name;
        enemyDetailDescription.text = enemyData.Description;
        enemyDetailTypeText.text = $"Type: {enemyData.Type}";
        enemyDetailContainer.SetActive(true);

        if (enemyData.HasEvolution && enemyData.EvolutionData != null)
        {
            evolutionHolder.SetActive(true);
            evolutionPreviewImage.sprite = enemyData.EvolutionData.Icon;

            evolutionButton.onClick.RemoveAllListeners();
            evolutionButton.onClick.AddListener(() =>
            {
                OpenEnemyDetailView(enemyData.EvolutionData);
            });
        }
        else
        {
            evolutionHolder.SetActive(false);
        }
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

    public void ShowNextEnemy()
    {
        if (loadedEnemies.Count == 0) return;
        currentEnemyIndex = (currentEnemyIndex + 1) % loadedEnemies.Count;
        OpenEnemyDetailView(loadedEnemies[currentEnemyIndex]);
    }

    public void ShowPreviousEnemy()
    {
        if (loadedEnemies.Count == 0) return;
        currentEnemyIndex = (currentEnemyIndex - 1 + loadedEnemies.Count) % loadedEnemies.Count;
        OpenEnemyDetailView(loadedEnemies[currentEnemyIndex]);
    }

    public void ShowNextCharacter()
    {
        if (loadedCharacters.Count == 0) return;
        currentCharacterIndex = (currentCharacterIndex + 1) % loadedCharacters.Count;
        OpenDetailView(loadedCharacters[currentCharacterIndex]);
    }

    public void ShowPreviousCharacter()
    {
        if (loadedCharacters.Count == 0) return;
        currentCharacterIndex = (currentCharacterIndex - 1 + loadedCharacters.Count) % loadedCharacters.Count;
        OpenDetailView(loadedCharacters[currentCharacterIndex]);
    }

    public void ShowNextWeapon()
    {
        if (loadedWeapons == null || loadedWeapons.Count == 0) return;

        currentWeaponIndex = (currentWeaponIndex + 1) % loadedWeapons.Count;
        OpenWeaponDetailView(loadedWeapons[currentWeaponIndex]);
    }

    public void ShowPreviousWeapon()
    {
        if (loadedWeapons == null || loadedWeapons.Count == 0) return;

        currentWeaponIndex = (currentWeaponIndex - 1 + loadedWeapons.Count) % loadedWeapons.Count;
        OpenWeaponDetailView(loadedWeapons[currentWeaponIndex]);
    }

    public void ShowNextObject()
    {
        if (loadedObjects == null || loadedObjects.Count == 0) return;

        currentObjectIndex = (currentObjectIndex + 1) % loadedObjects.Count;
        OpenObjectDetailView(loadedObjects[currentObjectIndex]);
    }

    public void ShowPreviousObject()
    {
        if (loadedObjects == null || loadedObjects.Count == 0) return;

        currentObjectIndex = (currentObjectIndex - 1 + loadedObjects.Count) % loadedObjects.Count;
        OpenObjectDetailView(loadedObjects[currentObjectIndex]);
    }

    public void ShowNextCard()
    {
        if (loadedCards.Count == 0) return;
        currentCardIndex = (currentCardIndex + 1) % loadedCards.Count;
        OpenCardDetail(loadedCards[currentCardIndex]);
    }

    public void ShowPreviousCard()
    {
        if (loadedCards.Count == 0) return;
        currentCardIndex = (currentCardIndex - 1 + loadedCards.Count) % loadedCards.Count;
        OpenCardDetail(loadedCards[currentCardIndex]);
    }

}
