using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SouthsideGames.SaveManager;
using System;

public class CharacterSelectionManager : MonoBehaviour, IWantToBeSaved
{
public static Action<CharacterDataSO> OnCharacterSelected;

    [Header("ELEMENTS:")]
    [SerializeField] private Transform characterButtonParent;
    [SerializeField] private CharacterInfoPanelUI characterInfo;

    [Header("CHARACTER FRAMES BY RARITY")]
    [SerializeField] private List<CharacterCardFrameMapping> characterFramesByRarity;

    [Header("SETTINGS")]
    [SerializeField] private float scrollSpeed;

    private CharacterDataSO[] characterDatas;
    private List<bool> characterUnlockStates = new List<bool>();    
    private int selectedCharacterIndex;
    private int lastSelectedCharacterIndex;
    private const string characterUnlockedStatesKey = "CharacterUnlockStatesKey";
    private const string lastSelectedCharacterKey = "LastSelectedCharacterKey";

    private Dictionary<CharacterCardRarityType, GameObject> characterFrameDictionary;
    private HashSet<string> spawnedCharacterIDs = new HashSet<string>();

    private CharacterContainerUI lastSelectedCharacter; // Track selected character

    public static CharacterSelectionManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this; // Singleton reference
        InputManager.OnScroll += ScrollCallback;
    }

    private void Start()
    {
        InitializeCharacterFrames();
        characterInfo.Button.onClick.RemoveAllListeners();
        characterInfo.Button.onClick.AddListener(PurchaseSelectedCharacter);
        Load();
        if (characterButtonParent.childCount == 0)
            Initialize();

        // Highlight the last selected character
        CharacterContainerUI selectedUI = characterButtonParent.GetChild(lastSelectedCharacterIndex).GetComponent<CharacterContainerUI>();
        SelectCharacter(selectedUI);
    }

    private void OnDestroy()
    {
        InputManager.OnScroll -= ScrollCallback;
    }

    private void InitializeCharacterFrames()
    {
        characterFrameDictionary = new Dictionary<CharacterCardRarityType, GameObject>();
        foreach (CharacterCardFrameMapping mapping in characterFramesByRarity)
        {
            if (!characterFrameDictionary.ContainsKey(mapping.rarity))
                characterFrameDictionary.Add(mapping.rarity, mapping.characterCardPrefab);
        }
    }

    private void Initialize()
    {
        spawnedCharacterIDs.Clear();
        foreach (Transform child in characterButtonParent)
            Destroy(child.gameObject);

        for (int i = 0; i < characterDatas.Length; i++)
            CreateCharacterButton(i);
    }

    private void CreateCharacterButton(int _index)
    {
        CharacterDataSO characterData = characterDatas[_index];

        if (spawnedCharacterIDs.Contains(characterData.ID)) return;
        spawnedCharacterIDs.Add(characterData.ID);

        if (!characterFrameDictionary.TryGetValue(characterData.Rarity, out GameObject prefabToUse))
            prefabToUse = characterFrameDictionary[CharacterCardRarityType.Common];

        GameObject characterButtonGO = Instantiate(prefabToUse, characterButtonParent);
        CharacterContainerUI characterButtonInstance = characterButtonGO.GetComponent<CharacterContainerUI>();

        bool isSelected = (_index == lastSelectedCharacterIndex); // Is this the saved selection?
        characterButtonInstance.ConfigureCharacterButton(characterData, characterUnlockStates[_index], characterInfo, isSelected);
    }

    public void SelectCharacter(CharacterContainerUI selectedCharacter)
    {
        if (lastSelectedCharacter != null)
        {
            lastSelectedCharacter.SetSelected(false); // Deselect previous character
        }

        lastSelectedCharacter = selectedCharacter;
        lastSelectedCharacter.SetSelected(true); // Highlight selected character

        // Save selection for persistence
        selectedCharacterIndex = System.Array.IndexOf(characterDatas, lastSelectedCharacter.characterData);
        Save();
    }

    private void CharacterSelectCallback(int _index)
    {
        selectedCharacterIndex = _index;
        CharacterDataSO characterData = characterDatas[_index];

        if (characterUnlockStates[_index])
        {
            lastSelectedCharacterIndex = _index;
            characterInfo.Button.interactable = false;
            Save();
            OnCharacterSelected.Invoke(characterData);  
        }
        else
            characterInfo.Button.interactable = CurrencyManager.Instance.HasEnoughPremiumCurrency(characterData.PurchasePrice);

        characterInfo.ConfigureInfoPanel(characterData, characterUnlockStates[_index]);
    }

    private void PurchaseSelectedCharacter()
    {
        int price = characterDatas[selectedCharacterIndex].PurchasePrice;
        CurrencyManager.Instance.AdjustPremiumCurrency(-price);
        characterUnlockStates[selectedCharacterIndex] = true;   
        characterButtonParent.GetChild(selectedCharacterIndex).GetComponent<CharacterContainerUI>().Unlock();
        CharacterSelectCallback(selectedCharacterIndex);
        Save();
    }

    public void Load()
    {
        characterDatas = ResourceManager.Characters;

        HashSet<string> idCheck = new HashSet<string>();
        for (int i = 0; i < characterDatas.Length; i++)
        {
            if (idCheck.Contains(characterDatas[i].ID) || string.IsNullOrEmpty(characterDatas[i].ID))
                continue;

            idCheck.Add(characterDatas[i].ID);
            characterUnlockStates.Add(i == 0); // First character is always unlocked
        }

        if (SaveManager.TryLoad(this, characterUnlockedStatesKey, out object characterUnlockedStatesObject))
            characterUnlockStates = (List<bool>)characterUnlockedStatesObject;

        if (SaveManager.TryLoad(this, lastSelectedCharacterKey, out object lastSelectedCharacterStatesObject))
            lastSelectedCharacterIndex = (int)lastSelectedCharacterStatesObject;
    }

    public void Save()
    {
        SaveManager.Save(this, characterUnlockedStatesKey, characterUnlockStates);
        SaveManager.Save(this, lastSelectedCharacterKey, lastSelectedCharacterIndex);
    }

    private void ScrollCallback(float _xValue) => characterButtonParent.GetComponent<RectTransform>().anchoredPosition -= _xValue * scrollSpeed * Time.deltaTime * Vector2.right;
}

[Serializable]
public class CharacterCardFrameMapping
{
    public CharacterCardRarityType rarity;
    public GameObject characterCardPrefab;
}
