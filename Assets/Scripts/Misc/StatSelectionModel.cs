using UnityEngine;
using TMPro;
using System.Linq;

public class StatSelectionModal : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown statDropdown;

    private int slotIndex;
    private StatBoosterSO booster;

    public void Show(int index, StatBoosterSO selectedBooster)
    {
        slotIndex = index;
        booster = selectedBooster;
        gameObject.SetActive(true);

        statDropdown.ClearOptions();
        statDropdown.AddOptions(System.Enum.GetNames(typeof(Stat)).ToList());
        statDropdown.value = 0;
    }

    public void ConfirmStatSelection()
    {
        Stat chosenStat = (Stat)statDropdown.value;
        CharacterManager.Instance.equipment.AssignBoosterToSlot(slotIndex, booster, chosenStat);
        gameObject.SetActive(false);
    }
}