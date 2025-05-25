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
    [SerializeField] private Image characterSelectImage;
    [SerializeField] private CharacterInfoPanelUI characterInfo;

    [Header("CARD FRAMES BY RARITY")]
    [SerializeField] private List<CharacterCardFrameMapping> characterCardFramesByRarity;

    [Header("SETTINGS")]
    [SerializeField] private float scrollSpeed;

    private CharacterDataSO[] characterDatas;
    private List<bool> characterUnlockStates = new List<bool>();
    private int selectedCharacterIndex;
    private int lastSelectedCharacterIndex;

    private Dictionary<CharacterRarityType, GameObject> characterCardFrameDictionary;

    private const string characterUnlockedStatesKey = "CharacterUnlockStatesKey";
    private const string lastSelectedCharacterKey = "LastSelectedCharacterKey";

    private void Awake()
    {
        InputManager.OnScroll += ScrollCallback;
        InitializeCharacterFrames();
        Load();
    }

    private void Start()
    {
        characterInfo.Button.onClick.RemoveAllListeners();
        characterInfo.Button.onClick.AddListener(PurchaseSelectedCharacter);

        if (characterDatas.Length > 0)
        {
            Initialize();
            CharacterSelectCallback(lastSelectedCharacterIndex);
        }
    }

    private void OnDestroy()
    {
        InputManager.OnScroll -= ScrollCallback;
    }

    private void InitializeCharacterFrames()
    {
        characterCardFrameDictionary = new Dictionary<CharacterRarityType, GameObject>();
        foreach (CharacterCardFrameMapping mapping in characterCardFramesByRarity)
        {
            if (!characterCardFrameDictionary.ContainsKey(mapping.rarity))
                characterCardFrameDictionary.Add(mapping.rarity, mapping.CharacterCardFramePrefab);
        }
    }

    private void Initialize()
    {
        List<int> sortedIndices = new List<int>();

        for (int i = 0; i < characterDatas.Length; i++)
        {
            if (characterUnlockStates[i])
                sortedIndices.Add(i);
        }

        for (int i = 0; i < characterDatas.Length; i++)
        {
            if (!characterUnlockStates[i])
                sortedIndices.Add(i);
        }

        for (int i = 0; i < sortedIndices.Count; i++)
            CreateCharacterButton(sortedIndices[i]);
    }

    private void CreateCharacterButton(int _index)
    {
        CharacterDataSO characterData = characterDatas[_index];

        if (!characterCardFrameDictionary.TryGetValue(characterData.Rarity, out GameObject framePrefab))
            return;

        GameObject frameInstance = Instantiate(framePrefab, characterButtonParent);

        CharacterContainerUI characterButtonInstance = frameInstance.GetComponent<CharacterContainerUI>();
        if (characterButtonInstance == null)
            return;

        characterButtonInstance.ConfigureCharacterButton(characterData.Icon, characterData.Name, characterUnlockStates[_index]);
        characterButtonInstance.Button.onClick.RemoveAllListeners();
        characterButtonInstance.Button.onClick.AddListener(() => CharacterSelectCallback(_index));
    }

    private void CharacterSelectCallback(int _index)
    {
        
        selectedCharacterIndex = _index;
        CharacterDataSO characterData = characterDatas[_index];
        
        foreach (var card in CardLibrary.Instance.allCards)
            card.isUnlocked = false;

        foreach (string id in characterData.startingCards)
        {
            CardSO card = CardLibrary.Instance.GetCardByID(id);
            if (card != null) card.isUnlocked = true;
        }


        if (characterUnlockStates[_index])
        {
            lastSelectedCharacterIndex = _index;
            characterInfo.Button.interactable = false;
            characterSelectImage.sprite = characterData.Icon;
            Save();
            OnCharacterSelected?.Invoke(characterData);
        }
        else
        {

            characterInfo.Button.interactable = CurrencyManager.Instance.HasEnoughPremiumCurrency(characterData.PurchasePrice);
        }

        characterInfo.ConfigureInfoPanel(characterData, characterUnlockStates[_index]);
    }

    private void PurchaseSelectedCharacter()
    {
        int price = characterDatas[selectedCharacterIndex].PurchasePrice;
        CurrencyManager.Instance.UsePremiumCurrency(price);

        characterUnlockStates[selectedCharacterIndex] = true;
        
        characterButtonParent.GetChild(selectedCharacterIndex).GetComponent<CharacterContainerUI>().Unlock();
        CharacterSelectCallback(selectedCharacterIndex);
        Save();
    }

    public void Load()
    {
        characterDatas = ResourceManager.Characters;

        characterUnlockStates.Clear();
        for (int i = 0; i < characterDatas.Length; i++)
            characterUnlockStates.Add(i == 0);

        if (SaveManager.TryLoad(this, characterUnlockedStatesKey, out object characterUnlockedStatesObject))
        {
            List<bool> loadedUnlockStates = (List<bool>)characterUnlockedStatesObject;


            if (loadedUnlockStates.Count == characterDatas.Length)
                characterUnlockStates = loadedUnlockStates;
        }

        if (SaveManager.TryLoad(this, lastSelectedCharacterKey, out object lastSelectedCharacterStatesObject))
            lastSelectedCharacterIndex = (int)lastSelectedCharacterStatesObject;
        else
            lastSelectedCharacterIndex = 0;
    }

    public void Save()
    {
        SaveManager.Save(this, characterUnlockedStatesKey, characterUnlockStates);
        SaveManager.Save(this, lastSelectedCharacterKey, lastSelectedCharacterIndex);
    }

    private void ScrollCallback(float _xValue)
    {
        characterButtonParent.GetComponent<RectTransform>().anchoredPosition -= _xValue * scrollSpeed * Time.deltaTime * Vector2.right;
    }
}

[Serializable]
public class CharacterCardFrameMapping
{
    public CharacterRarityType rarity;
    public GameObject CharacterCardFramePrefab;
}