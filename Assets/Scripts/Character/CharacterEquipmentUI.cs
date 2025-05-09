using System.Collections.Generic;
using UnityEngine;

public class CharacterEquipmentUI : MonoBehaviour
{
    [SerializeField] private List<BoosterSlotUI> boosterSlots;
    [SerializeField] private BoosterSelectionUI boosterSelectionUI;

    private int selectedSlotIndex = -1;

    private void Start()
    {
        for (int i = 0; i < boosterSlots.Count; i++)
        {
            int index = i;
            boosterSlots[i].Initialize(index, this);
        }
    }

    public void OpenBoosterSelection(int slotIndex)
    {
        selectedSlotIndex = slotIndex;
        boosterSelectionUI.gameObject.SetActive(true);
        boosterSelectionUI.ShowAvailableBoosters(this);
    }

    public void EquipBooster(StatBoosterSO booster)
    {
        if (selectedSlotIndex < 0 || selectedSlotIndex >= boosterSlots.Count) return;

        // Assign temporarily with default stat (stat selection happens after)
        CharacterManager.Instance.equipment.AssignBoosterToSlot(selectedSlotIndex, booster, Stat.Attack);
        boosterSlots[selectedSlotIndex].UpdateDisplay(booster);
        boosterSelectionUI.gameObject.SetActive(false);

        // TODO: open stat selection modal here
    }
}