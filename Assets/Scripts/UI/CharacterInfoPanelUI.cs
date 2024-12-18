using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CharacterInfoPanelUI : MonoBehaviour
{
    [Header("ELEMENTS")]
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI priceText;
    [SerializeField] private GameObject priceContainer;
    [SerializeField] private Transform statsParent;

    [field: SerializeField] public Button Button {get; private set;}    

    public void ConfigureInfoPanel(CharacterDataSO _characterDataSO, bool unlocked)
    {
        nameText.text = _characterDataSO.Name;
        priceText.text = _characterDataSO.PurchasePrice.ToString(); 

        priceContainer.SetActive(!unlocked);

        StatContainerManager.GenerateStatContainers(_characterDataSO.NonNeutralStats, statsParent);

    }
}
