using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class CodexCardUI : MonoBehaviour
{
    [Header("ELEMENTS")]
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI cardName;

    private Button cardButton;
    private CharacterDataSO characterData;
    private WeaponDataSO weaponData;
    private ObjectDataSO objectData;
    private EnemyDataSO enemyData;
    private CodexManager codexManager;

    private void Awake() 
    {
        cardButton = GetComponent<Button>();    
    }


    public void InitializeCharacterCard(Sprite _icon, string _name, CharacterDataSO _data, CodexManager _manager)
    {
        AssignData(_icon, _name, _manager);
        characterData = _data;
        cardButton.onClick.AddListener(() => codexManager.OpenDetailView(characterData));
    }

    public void InitializeWeaponCard(Sprite _icon, string _name, WeaponDataSO _data, CodexManager _manager)
    {
        AssignData(_icon, _name, _manager);
        weaponData = _data;
        cardButton.onClick.AddListener(() => codexManager.OpenWeaponDetailView(weaponData));
    }

    public void InitializeObjectCard(Sprite _icon, string _name, ObjectDataSO _data, CodexManager _manager)
    {
        AssignData(_icon, _name, _manager);
        objectData = _data;
        cardButton.onClick.AddListener(() => codexManager.OpenObjectDetailView(objectData));
    }

    public void InitializeEnemyCard(Sprite _icon, string _name, EnemyDataSO _data, CodexManager _manager)
    {
        AssignData(_icon, _name, _manager);
        enemyData = _data;
        cardButton.onClick.AddListener(() => codexManager.OpenEnemyDetailView(enemyData));
    }

    private void AssignData(Sprite _icon, string _name, CodexManager _manager)
    {
        icon.sprite = _icon;
        cardName.text = _name;
        codexManager = _manager;

        cardButton.onClick.RemoveAllListeners();  // Avoid stacking listeners
    }

}
