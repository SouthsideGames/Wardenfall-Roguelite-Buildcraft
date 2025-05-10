using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BoosterSlotUI : MonoBehaviour
{
    [SerializeField] private Button slotButton;
    [SerializeField] private Image icon;
    [SerializeField] private Sprite defaultIcon;

    private int slotIndex;
    private CharacterEquipmentUI equipmentUI;

    public void Initialize(int index, CharacterEquipmentUI ui)
    {
        slotIndex = index;
        equipmentUI = ui;
        slotButton.onClick.AddListener(OnSlotClicked);
    }

    public void OnSlotClicked()
    {
        equipmentUI.OpenBoosterSelection(slotIndex);
    }

    public void UpdateDisplay(StatBoosterSO booster)
    {
        if (booster != null)
        {
            icon.sprite = booster.icon;
        }
        else
        {
            icon.sprite = defaultIcon;
        }
    }
}