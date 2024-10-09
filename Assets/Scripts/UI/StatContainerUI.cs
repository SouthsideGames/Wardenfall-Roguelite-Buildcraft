using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;   
using TMPro;


public class StatContainerUI : MonoBehaviour
{
    [Header("ELEMENTS:")]
    [SerializeField] private Image statImage;
    [SerializeField] private TextMeshProUGUI statNameText;
    [SerializeField] private TextMeshProUGUI statValueText;

    public void Configure(Sprite _icon, string _statName, string _statValue)
    {
        statImage.sprite = _icon;
        statNameText.text    = _statName;
        statValueText.text = _statValue;
    }

    public float GetFontSize()
    {
        return statNameText.fontSize;
    }

    public void SetFontSize(float _fontSize)
    {
        statNameText.fontSizeMax = _fontSize;  
        statValueText.fontSizeMax = _fontSize;  
    }
}
