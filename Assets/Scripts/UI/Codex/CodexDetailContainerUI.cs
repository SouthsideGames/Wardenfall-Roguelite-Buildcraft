using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CodexDetailContainerUI : MonoBehaviour
{
    [Header("Mini Card Elements")]
    [SerializeField] private GameObject miniCardPrefab;
    [SerializeField] private Transform miniCardParent;     
    [SerializeField] private CodexManager codexManager;

    public void ShowCharacterDetails() => LoadAndDisplayCharacterCards();
    public void ShowWeaponDetails() => LoadAndDisplayWeaponCards();
    public void ShowObjectDetails() => LoadAndDisplayObjectCards();
    public void ShowEnemyDetails() => LoadAndDisplayEnemyCards();


    private void LoadAndDisplayCharacterCards()
    {
        miniCardParent.Clear();

        CharacterDataSO[] characterDataItems = Resources.LoadAll<CharacterDataSO>("Data/Characters");
      
        foreach (CharacterDataSO characterData in characterDataItems)
        {
            GameObject miniCard = Instantiate(miniCardPrefab, miniCardParent);
            
            CodexMiniCardUI miniCardUI = miniCard.GetComponent<CodexMiniCardUI>();
            miniCardUI.Initialize(characterData.Icon, characterData.Name, characterData.Description, codexManager);
        }
    }

    private void LoadAndDisplayWeaponCards()
    {
        miniCardParent.Clear();

        WeaponDataSO[] weaponDataItems = Resources.LoadAll<WeaponDataSO>("Data/Weapons");
      
        foreach (WeaponDataSO weaponData in weaponDataItems)
        {
            GameObject miniCard = Instantiate(miniCardPrefab, miniCardParent);
            
            CodexMiniCardUI miniCardUI = miniCard.GetComponent<CodexMiniCardUI>();
            miniCardUI.Initialize(weaponData.Icon, weaponData.Name, weaponData.Description, codexManager);
        }
    }

    private void LoadAndDisplayObjectCards()
    {
        miniCardParent.Clear();

        ObjectDataSO[] objectDataItems = Resources.LoadAll<ObjectDataSO>("Data/Objects");
      
        foreach (ObjectDataSO objectData in objectDataItems)
        {
            GameObject miniCard = Instantiate(miniCardPrefab, miniCardParent);
            
            CodexMiniCardUI miniCardUI = miniCard.GetComponent<CodexMiniCardUI>();
            miniCardUI.Initialize(objectData.Icon, objectData.Name, objectData.GetFullDescription(), codexManager);
        }
    }

    private void LoadAndDisplayEnemyCards()
    {
        miniCardParent.Clear();

        EnemyDataSO[] enemyDataItems = Resources.LoadAll<EnemyDataSO>("Data/Enemies");
      
        foreach (EnemyDataSO enemyData in enemyDataItems)
        {
            GameObject miniCard = Instantiate(miniCardPrefab, miniCardParent);
            
            CodexMiniCardUI miniCardUI = miniCard.GetComponent<CodexMiniCardUI>();
            miniCardUI.Initialize(enemyData.Icon, enemyData.Name, enemyData.Description, codexManager);
        }
    }

}
