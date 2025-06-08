using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BoosterUI : MonoBehaviour
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

        label.text = $"{booster.bonusValue}%";
        icon.sprite = booster.icon;

        bool canEquip = CharacterManager.Instance.equipment.CanEquipBooster(boosterData);
        selectButton.interactable = canEquip;

        selectButton.onClick.RemoveAllListeners();
        selectButton.onClick.AddListener(() =>
        {
            if (!canEquip)
            {
                Debug.LogWarning("Booster already equipped.");
                return;
            }

            equipmentUI.EquipBooster(boosterData);
        });
    }
}
