using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectionManager : MonoBehaviour
{
    [Header("ELEMENTS:")]
    [SerializeField] private CharacterContainerUI characterButtonPrefab;
    [SerializeField] private Transform characterButtonParent;
    [SerializeField] private Image characterSelectImage;
    [SerializeField] private CharacterInfoPanelUI characterInfo;

    private CharacterDataSO[] characterDatas;
    private List<bool> characterUnlockStates = new List<bool>();    
    private int selectedCharacterIndex;

    private void Awake() 
    {
        characterDatas = ResourceManager.Characters;

        //Makes the first character unlocked
        for (int i = 0; i < characterDatas.Length; i++)
            characterUnlockStates.Add(i == 0);
    }

    private void Start()
    {
        characterInfo.Button.onClick.RemoveAllListeners();
        characterInfo.Button.onClick.AddListener(PurchaseSelectedCharacter);

        Initialize();
    }

    private void Initialize()
    {
        for (int i = 0; i < characterDatas.Length; i++)
            CreateCharacterButton(i);
    }

    private void CreateCharacterButton(int _index)
    {
        CharacterDataSO characterData = characterDatas[_index];

        CharacterContainerUI characterButtonInstance = Instantiate(characterButtonPrefab, characterButtonParent);
        characterButtonInstance.ConfigureCharacterButton(characterData.Icon);

        characterButtonInstance.Button.onClick.RemoveAllListeners();
        characterButtonInstance.Button.onClick.AddListener(() =>CharacterSelectCallback(_index));
        characterButtonInstance.Button.onClick.AddListener(() =>StatisticsManager.Instance.RecordCharacterUsage(characterData.ID));
    }

    private void CharacterSelectCallback(int _index)
    {
        selectedCharacterIndex = _index;

        CharacterDataSO characterData = characterDatas[_index];

        if(characterUnlockStates[_index])
            characterInfo.Button.interactable = false;
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

        //Update the character info - hide purchase button
        CharacterSelectCallback(selectedCharacterIndex);
    }
}
