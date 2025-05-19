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

    public void Configure(Sprite _icon, string _statName, float _statValue, bool _shouldColor = false)
    {
        // Hide the image if no icon is provided
        if (_icon == null)
            statImage.gameObject.SetActive(false);
        else
        {
            statImage.gameObject.SetActive(true);
            statImage.sprite = _icon;
        }

        statNameText.text = _statName;

        if (_shouldColor)
            ColorizeStatValue(_statValue);
        else
        {
            statValueText.color = Color.white;
            statValueText.text = _statValue.ToString("F2");
        }
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

    private void ColorizeStatValue(float _statValue)
    {
        float sign = Mathf.Sign(_statValue);

        if (_statValue == 0)
            sign = 0;

        float absStatValue = Mathf.Abs(_statValue);

        Color statValueTextColor = Color.white;

        if (sign > 0)
            statValueTextColor = Color.green;
        else if (sign < 0)
            statValueTextColor = Color.red;

        statValueText.color = statValueTextColor;
        statValueText.text = absStatValue.ToString("F2");

    }
}
