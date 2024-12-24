using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CharacterInfoPanelUI : MonoBehaviour
{
    [Header("ELEMENTS")]
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI priceText;
    [SerializeField] private GameObject priceContainer;
    [SerializeField] private Transform statsParent;

    [Header("LEVEL INFO")]
    [SerializeField] private Slider experienceSlider;
    [SerializeField] private TextMeshProUGUI levelText;

    [field: SerializeField] public Button Button { get; private set; }

    public void ConfigureInfoPanel(CharacterDataSO _characterDataSO, bool unlocked, int level, int experience, int experienceToNextLevel)
    {
        // Set visuals
        icon.sprite = _characterDataSO.Icon;
        nameText.text = _characterDataSO.Name;
        priceText.text = _characterDataSO.PurchasePrice.ToString();
        priceContainer.SetActive(!unlocked);

        // Update Stats
        StatContainerManager.GenerateStatContainers(_characterDataSO.NonNeutralStats, statsParent);

        // Update Level UI
        levelText.text = level.ToString();
        float progress = (float)experience / experienceToNextLevel;
        experienceSlider.value = progress;
    }
}
