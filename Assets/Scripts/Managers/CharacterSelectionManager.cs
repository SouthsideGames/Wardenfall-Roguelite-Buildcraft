using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectionManager : MonoBehaviour
{
    [Header("ELEMENTS:")]
    [SerializeField] private CharacterContainerUI characterButtonPrefab;
    [SerializeField] private Transform characterButtonParent;

    private CharacterDataSO[] characterDatas;

    private void Awake() 
    {
        characterDatas = ResourceManager.Characters;
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
    }
}
