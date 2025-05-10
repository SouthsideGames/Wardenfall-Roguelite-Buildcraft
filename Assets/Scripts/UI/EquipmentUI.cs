using System.Collections.Generic;
using UnityEngine;

public class EquipmentUI : MonoBehaviour
{
   [SerializeField] private List<BoosterSlotUI> boosterSlots;
    [SerializeField] private BoosterSelectionUI boosterSelectionUI;
    [SerializeField] private StatSelectionModal statSelectionModal;

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

        boosterSelectionUI.gameObject.SetActive(false);

        statSelectionModal.Show(selectedSlotIndex, booster);
    }

    public void FinalizeBoosterAssignment(int slotIndex, StatBoosterSO booster, Stat chosenStat)
    {
        CharacterManager.Instance.equipment.AssignBoosterToSlot(slotIndex, booster, chosenStat);
        boosterSlots[slotIndex].UpdateDisplay(booster);
    }

    
}