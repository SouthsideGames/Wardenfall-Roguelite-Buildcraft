using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;   

public class CollectionContainerUI : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI usageCountText;
    [SerializeField] private TextMeshProUGUI lastUsedDateText;

    public void Configure(Sprite _icon, string _name, int _usageCount, DateTime _lastUsed)
    {
        iconImage.sprite = _icon;
        nameText.text = _name;  
        usageCountText.text = $"Times Used: {_usageCount}";
        lastUsedDateText.text = $"Last Used: {_lastUsed.ToShortDateString()}";
    }
}
