using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class InventoryItemContainerUI : MonoBehaviour
{
    [Header("ELEMENTS:")]
    [SerializeField] private Image container;
    [SerializeField] private Image icon;
    [SerializeField]private Button button;

    public int Index { get; private set; }

    public Weapon Weapon {get; private set;}    
    public ObjectDataSO ObjectData {get; private set;}  

    private void Awake() 
    {
        button = GetComponent<Button>();
    }

    public void Configure(Color _containerColor, Sprite _itemIcon)
    {
        container.color = _containerColor;  
        icon.sprite = _itemIcon;   
    }

    public void Configure(Weapon _weapon, int _index, Action _selectedCallback)
    {
        Weapon = _weapon;
        Index = _index;

        Color color = ColorHolder.GetColor(_weapon.Level);
        Sprite icon = _weapon.WeaponData.Icon;

        Configure(color, icon);

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => _selectedCallback?.Invoke());
    }

    public void Configure(ObjectDataSO _objectData, Action _selectedCallback)
    {
        ObjectData = _objectData;
        
        Color color = ColorHolder.GetColor(_objectData.Rarity);
        Sprite icon = _objectData.Icon;

        Configure(color, icon);

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => _selectedCallback?.Invoke());
    }
}
