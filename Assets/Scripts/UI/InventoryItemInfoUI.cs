using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

public class InventoryItemInfoUI : MonoBehaviour
{
    [Header("ELEMENTS:")]
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI itemNameText;
    [SerializeField] private TextMeshProUGUI recyclePriceText;

    [Header("COLORS:")]
    [SerializeField] private Image container;

    [Header("STATS:")]
    [SerializeField] private Transform statsParent;

    public void Configure(Weapon _weapon)
    {
        Configure
            (
                _weapon.WeaponData.Icon,
                _weapon.WeaponData.Name + "(lvl " +(_weapon.Level + 1)+")",
                ColorHolder.GetColor(_weapon.Level),
                WeaponStatCalculator.GetRecyclePrice(_weapon.WeaponData, _weapon.Level),
                WeaponStatCalculator.GetStats(_weapon.WeaponData, _weapon.Level)

            );
    }

    public void Configure(ObjectDataSO _objectData)
    {
        Configure
            (
                _objectData.Icon,
                _objectData.Name,
                ColorHolder.GetColor(_objectData.Rarity),
                _objectData.RecyclePrice,
                _objectData.BaseStats

            );
    }

    private void Configure(Sprite _icon, string _name, Color _containerColor, int _recyclePrice, Dictionary<Stat, float> stats)
    {
        icon.sprite = _icon;
        itemNameText.text = _name;    
        itemNameText.color = _containerColor;

        recyclePriceText.text = _recyclePrice.ToString();

        container.color = _containerColor;

        StatContainerManager.GenerateStatContainers(stats, statsParent);

    }
}
