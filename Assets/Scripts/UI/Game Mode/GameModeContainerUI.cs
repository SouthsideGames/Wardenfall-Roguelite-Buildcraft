using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameModeContainerUI : MonoBehaviour
{
    
    [Header("ELEMENTS:")]
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI gameModeNameText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    public void Configure(GameModeDataSO _gameModeData, bool _isUnlocked)
    {
        icon.sprite = _gameModeData.Icon;
        gameModeNameText.text = _gameModeData.Name;
        descriptionText.text = _gameModeData.Description;

    }

}
