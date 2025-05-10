using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class BoosterCardUI : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI label;
    [SerializeField] private Button selectButton;

    private StatBoosterSO boosterData;
    private EquipmentUI equipmentUI;

    public void Configure(StatBoosterSO booster, EquipmentUI ui)
    {
        boosterData = booster;
        equipmentUI = ui;

        label.text = booster.name;
        icon.sprite = booster.icon;

        selectButton.onClick.RemoveAllListeners();
        selectButton.onClick.AddListener(OnSelected);
    }

    private void OnSelected()
    {
        equipmentUI.EquipBooster(boosterData);
    }
}
