using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;    

[RequireComponent(typeof(Button))]
public class UpgradeContainerUI : MonoBehaviour
{
    [Header("ELEMENTS:")]
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI upgradeNameText;
    [SerializeField] private TextMeshProUGUI upgradeValueText;

    [field: SerializeField] public Button Button { get; private set; }    
    
    public void Configure(Sprite _icon, string _upgradeName, string _upgradeValue)
    {
        icon.sprite = _icon;
        upgradeNameText.text = _upgradeName;    
        upgradeValueText.text = _upgradeValue;
    }

}
