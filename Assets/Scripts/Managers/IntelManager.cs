using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class IntelManager : MonoBehaviour
{
    [Header("ELEMENTS:")]
    [SerializeField] private TMP_Dropdown categoryDropdown;
    [SerializeField] private Transform detailParent;
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private TextMeshProUGUI progressText;
    [SerializeField] private CardLibrary cardLibrary;

    [Header("CHARACTER DETAIL VIEW:")]
    [SerializeField] private GameObject characterDetailContainer;
    [SerializeField] private Image characterDetailIcon;
    [SerializeField] private TextMeshProUGUI characterDetailName;
    [SerializeField] private TextMeshProUGUI characterDetailDescription;
    [SerializeField] private Image abilityIcon;
    [SerializeField] private TextMeshProUGUI abilityName;
    [SerializeField] private TextMeshProUGUI abilityDescription;
    [SerializeField] private Transform characterStatContainersParent;

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
    [SerializeField] private GameObject statPrefab;

    [Header("WEAPON VIEW:")]
    [SerializeField] private GameObject weaponDetailContainer;
    [SerializeField] private Image weaponDetailIcon;
    [SerializeField] private TextMeshProUGUI weaponDetailName;
    [SerializeField] private TextMeshProUGUI weaponDetailDescription;
    [SerializeField] private TextMeshProUGUI weaponIdText;
    [SerializeField] private TextMeshProUGUI recycleValueText;
    [SerializeField] private Transform weaponStatContainersParent;


    private List<EnemyDataSO> loadedEnemies = new();
    private int currentEnemyIndex = 0;
    private List<CharacterDataSO> loadedCharacters = new();
    private int currentCharacterIndex = 0;
    private List<WeaponDataSO> loadedWeapons = new();
    private int currentWeaponIndex = 0;
    private List<ObjectDataSO> loadedObjects = new();
    private int currentObjectIndex = 0;

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
        var categories = new List<string> { "Characters", "Weapons", "Objects", "Enemies"};
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
            default: Debug.LogWarning("Invalid category selected."); break;
        }
    }

#region Card Loading and Display

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

    #endregion

#region Open Detail Views
    public void OpenCharacterDetailView(CharacterDataSO _characterData)
    {
        CloseDetailView();

        characterDetailIcon.sprite = _characterData.Icon;
        characterDetailName.text = _characterData.Name;
        characterDetailDescription.text = _characterData.Description;
        abilityIcon.sprite = _characterData.AbilityIcon;
        abilityName.text = _characterData.AbilityName;
        abilityDescription.text = _characterData.AbilityDescription;
        DisplayCharacterStats(_characterData);
        characterDetailContainer.SetActive(true);
        currentCharacterIndex = loadedCharacters.IndexOf(_characterData);
    }

    public void OpenWeaponDetailView(WeaponDataSO _weaponData)
    {
        weaponDetailIcon.sprite = _weaponData.Icon;
        weaponDetailName.text = _weaponData.Name;
        weaponDetailDescription.text = _weaponData.Description;
        weaponIdText.text = $"ID: {_weaponData.ID}";
        recycleValueText.text = $"Recycle Value: {_weaponData.RecyclePrice.ToString()}";
        weaponStatContainersParent.Clear();
        DisplayWeaponStats(_weaponData);
        weaponDetailContainer.SetActive(true);
        currentWeaponIndex = loadedWeapons.IndexOf(_weaponData);
    }


    public void OpenObjectDetailView(ObjectDataSO _objectData)
    {
        objectDetailIcon.sprite = _objectData.Icon;
        objectDetailName.text = _objectData.Name;
        characterStatContainersParent.Clear();
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

    #endregion

#region Display Stats
    private void DisplayCharacterStats(CharacterDataSO characterData)
    {
        characterStatContainersParent.Clear();
        foreach (var stat in characterData.NonNeutralStats)
        {
            GameObject statEntry = Instantiate(statPrefab, characterStatContainersParent);
            StatContainerUI statContainerUI = statEntry.GetComponent<StatContainerUI>();
            Sprite statIcon = ResourceManager.GetStatIcon(stat.Key);
            statContainerUI.Configure(statIcon, Enums.FormatStatName(stat.Key), stat.Value, true);
        }
    }

   private void DisplayWeaponStats(WeaponDataSO weaponData)
    {
        weaponStatContainersParent.Clear();

        foreach (var stat in weaponData.BaseStats)
        {
            GameObject statEntry = Instantiate(statPrefab, weaponStatContainersParent);
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

#endregion

    public void CloseCharacterDetailView() => characterDetailContainer.SetActive(false);
    public void CloseEnemyDetailView() => enemyDetailContainer.SetActive(false);
    public void CloseObjectDetailView() => objectDetailContainer.SetActive(false);
    public void CloseWeaponDetailView() => weaponDetailContainer.SetActive(false);
    private void ClearCards() => detailParent.Clear();

    private void CloseDetailView()
    {
        characterDetailContainer.SetActive(false);
        enemyDetailContainer.SetActive(false);
        objectDetailContainer.SetActive(false);
        weaponDetailContainer.SetActive(false);
    }

#region Navigation Methods

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
        OpenCharacterDetailView(loadedCharacters[currentCharacterIndex]);
    }

    public void ShowPreviousCharacter()
    {
        if (loadedCharacters.Count == 0) return;
        currentCharacterIndex = (currentCharacterIndex - 1 + loadedCharacters.Count) % loadedCharacters.Count;
        OpenCharacterDetailView(loadedCharacters[currentCharacterIndex]);
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
#endregion
   

}
