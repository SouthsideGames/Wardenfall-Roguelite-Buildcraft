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

        RefreshEquippedBoostersUI();
    }


    public void OpenBoosterSelection(int slotIndex)
    {
        selectedSlotIndex = slotIndex;
        boosterSelectionUI.gameObject.SetActive(true);
        boosterSelectionUI.ShowAvailableBoosters(this);
    }

    public void CloseBoosterSelection()
    {

        boosterSelectionUI.gameObject.SetActive(false);
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
        CharacterManager.Instance.stats.ApplyProgressionBoosters(CharacterManager.Instance.equipment.equippedBoosters);
    }


    private void RefreshEquippedBoostersUI()
    {
        var equipment = CharacterManager.Instance.equipment;

        for (int i = 0; i < boosterSlots.Count; i++)
        {
            if (i < equipment.equippedBoosters.Count && equipment.equippedBoosters[i].booster != null)
            {
                boosterSlots[i].UpdateDisplay(equipment.equippedBoosters[i].booster);
            }
            else
            {
                boosterSlots[i].UpdateDisplay(null); // Clear if no booster
            }
        }
    }

    public void ClearAllBoosters()
    {
        var equipment = CharacterManager.Instance.equipment;
        equipment.equippedBoosters.Clear();
        equipment.SaveEquippedBoosters();

        RefreshEquippedBoostersUI();
    }


    
}