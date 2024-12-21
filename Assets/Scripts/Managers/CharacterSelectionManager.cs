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
    [SerializeField] private CharacterContainerUI characterCardPrefab;
    [SerializeField] private Transform characterButtonParent;
    [SerializeField] private Image characterSelectImage;
    [SerializeField] private CharacterInfoPanelUI characterInfo;

    [Header("SETTINGS")]
    [SerializeField] private float scrollSpeed;

    private CharacterDataSO[] characterDatas;
    private List<bool> characterUnlockStates = new List<bool>();    
    private int selectedCharacterIndex;
    private int lastSelectedCharacterIndex;
    private const string characterUnlockedStatesKey = "CharacterUnlockStatesKey";
    private const string lastSelectedCharacterKey = "LastSelectedCharacterKey";

    private void Awake() => InputManager.OnScroll += ScrollCallback;
    private void Start()
    {
        characterInfo.Button.onClick.RemoveAllListeners();
        characterInfo.Button.onClick.AddListener(PurchaseSelectedCharacter);

        CharacterSelectCallback(lastSelectedCharacterIndex);
    }

    private void OnDestroy() => InputManager.OnScroll -= ScrollCallback;

    private void Initialize()
    {
        for (int i = 0; i < characterDatas.Length; i++)
            CreateCharacterButton(i);
    }

    private void CreateCharacterButton(int _index)
    {
        CharacterDataSO characterData = characterDatas[_index];

        CharacterContainerUI characterButtonInstance = Instantiate(characterCardPrefab, characterButtonParent);
        characterButtonInstance.ConfigureCharacterButton(characterData.Icon, characterUnlockStates[_index]);

        characterButtonInstance.Button.onClick.RemoveAllListeners();
        characterButtonInstance.Button.onClick.AddListener(() =>CharacterSelectCallback(_index));
    }

    private void CharacterSelectCallback(int _index)
    {
        selectedCharacterIndex = _index;

        CharacterDataSO characterData = characterDatas[_index];

        if(characterUnlockStates[_index])
        {
            lastSelectedCharacterIndex = _index;
            characterInfo.Button.interactable = false;
            Save();

            OnCharacterSelected.Invoke(characterData);  
        }
        else
            characterInfo.Button.interactable = CurrencyManager.Instance.HasEnoughPremiumCurrency(characterData.PurchasePrice);

        characterSelectImage.sprite = characterData.Icon;   
        characterInfo.ConfigureInfoPanel(characterData, characterUnlockStates[_index]);
    }

    private void PurchaseSelectedCharacter()
    {
        int price = characterDatas[selectedCharacterIndex].PurchasePrice;
        CurrencyManager.Instance.AdjustPremiumCurrency(-price);

        // Save Unlock state of that Character
        characterUnlockStates[selectedCharacterIndex] = true;   

        //Update character visuals
        characterButtonParent.GetChild(selectedCharacterIndex).GetComponent<CharacterContainerUI>().Unlock();

        //Update the character info - hide purchase button
        CharacterSelectCallback(selectedCharacterIndex);

        Save();
    }

    public void Load()
    {
        characterDatas = ResourceManager.Characters;

        //Makes the first character unlocked
        for (int i = 0; i < characterDatas.Length; i++)
            characterUnlockStates.Add(i == 0);

        if(SaveManager.TryLoad(this, characterUnlockedStatesKey, out object characterUnlockedStatesObject))
            characterUnlockStates = (List<bool>)characterUnlockedStatesObject;

        //load the last character we played with
        if(SaveManager.TryLoad(this, lastSelectedCharacterKey, out object lastSelectedCharacterStatesObject))
            lastSelectedCharacterIndex = (int)lastSelectedCharacterStatesObject;

        Initialize();
    }

    public void Save()
    {
        SaveManager.Save(this, characterUnlockedStatesKey, characterUnlockStates);
        SaveManager.Save(this, lastSelectedCharacterKey, lastSelectedCharacterIndex);
    }

    private void ScrollCallback(float _xValue) => characterButtonParent.GetComponent<RectTransform>().anchoredPosition -= _xValue * scrollSpeed * Time.deltaTime * Vector2.right;
}
