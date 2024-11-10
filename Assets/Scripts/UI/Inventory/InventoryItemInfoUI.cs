using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

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

    [Header("BUTTONS:")]
    [field: SerializeField] public Button RecycleButton {get; private set;}
    [SerializeField] private Button fuseButton;

    public void ConfigureInventoryInfo(Weapon _weapon)
    {
        Configure
            (
                _weapon.WeaponData.Icon,
                _weapon.WeaponData.Name + "(lvl " +(_weapon.Level + 1)+")",
                ColorHolder.GetColor(_weapon.Level),
                WeaponStatCalculator.GetRecyclePrice(_weapon.WeaponData, _weapon.Level),
                WeaponStatCalculator.GetStats(_weapon.WeaponData, _weapon.Level)

            );

        fuseButton.gameObject.SetActive(true);

        fuseButton.interactable = WeaponFuserManager.Instance.CanFuse(_weapon);

        fuseButton.onClick.RemoveAllListeners();
        fuseButton.onClick.AddListener(WeaponFuserManager.Instance.Fuse);
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
        
        fuseButton.gameObject.SetActive(false);
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
