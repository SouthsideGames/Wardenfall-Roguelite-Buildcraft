using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CodexBigCardDetailUI : MonoBehaviour
{
   [Header("UI Elements")]
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI descriptionText;

    public void Configure(Sprite icon, string name, string description)
    {
        iconImage.sprite = icon;
        nameText.text = name;
        descriptionText.text = description;
    }
}
