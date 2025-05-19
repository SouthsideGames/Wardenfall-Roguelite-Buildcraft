using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class StatisticsContainerUI : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI usageCountText;
    [SerializeField] private TextMeshProUGUI wavesCompletedText;
    [SerializeField] private TextMeshProUGUI lastUsedText;

    public void Configure(Sprite _icon, string _name, int _usageCount, int _wavesCompleted, DateTime _lastUsed)
    {
        icon.sprite = _icon;
        nameText.text = _name;
        usageCountText.text = $"Used: {_usageCount} times";
        wavesCompletedText.text = $"Waves: {_wavesCompleted}";
        lastUsedText.text = $"Last Used: {_lastUsed:g}";
    }
}
