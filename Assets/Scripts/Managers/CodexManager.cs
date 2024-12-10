using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CodexManager : MonoBehaviour
{

   
    [Header("ELEMENTS:")]
    [SerializeField] private TMP_Dropdown categoryDropdown;
    [SerializeField] private Transform detailParent;
    [SerializeField] private GameObject cardPrefab;

    [Header("DETAIL VIEW:")]
    [SerializeField] private GameObject detailContainer;
    [SerializeField] private Image detailIcon;
    [SerializeField] private TextMeshProUGUI detailName;
    [SerializeField] private TextMeshProUGUI detailDescription;
    [SerializeField] private Transform statContainersParent;
    [SerializeField] private GameObject statPrefab;

    private void Awake()
    {
        InitializeDropdown();
        LoadAndDisplayCharacterCards();
    }

    private void Start() => CloseDetailView();
    private void InitializeDropdown()
    {
        categoryDropdown.ClearOptions();
        var categories = new System.Collections.Generic.List<string> { "Characters", "Weapons", "Objects", "Enemies" };
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
            default:
                Debug.LogWarning("Invalid category selected.");
                break;
        }
    }

    #region Card Loading Methods

    private void LoadAndDisplayCharacterCards()
    {
        ClearCards();
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
        detailIcon.sprite = _objectData.Icon;
        detailName.text = _objectData.Name;
        detailDescription.text = _objectData.Description;
        statContainersParent.Clear();
        DisplayObjectStats(_objectData);
        detailContainer.SetActive(true);
    }

    public void OpenEnemyDetailView(EnemyDataSO _enemyData)
    {
        detailIcon.sprite = _enemyData.Icon;
        detailName.text = _enemyData.Name;
        detailDescription.text = _enemyData.Description;
        statContainersParent.Clear();
        detailContainer.SetActive(true);
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
        statContainersParent.Clear();

        foreach (var stat in _objectData.BaseStats)
        {
            GameObject statEntry = Instantiate(statPrefab, statContainersParent);
            StatContainerUI statContainerUI = statEntry.GetComponent<StatContainerUI>();
            Sprite statIcon = ResourceManager.GetStatIcon(stat.Key);
            statContainerUI.Configure(statIcon, Enums.FormatStatName(stat.Key), stat.Value, true);
        }
    }

    public void CloseDetailView() => detailContainer.SetActive(false);

    private void ClearCards() => detailParent.Clear();

    #endregion

}
