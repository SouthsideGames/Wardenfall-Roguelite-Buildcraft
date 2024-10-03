using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WeaponSelectionContainerUI : MonoBehaviour
{
    [Header("ELEMENTS:")]
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI weaponNameText;

    [field: SerializeField] public Button Button { get; private set; }    

    public void Configure(Sprite _icon, string _weaponName)
    {
        icon.sprite = _icon;
        weaponNameText.text = _weaponName;

    }

}
