using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class ChestObjectContainerUI : MonoBehaviour
{
    public static Action<GameObject> OnSpawned;

    [Header("ELEMENTS:")]
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI nameText;

    [Header("STATS:")]
    [SerializeField] private Transform statContainerParent;

    [field: SerializeField] public Button CollectButton { get; private set; }    
    [field: SerializeField] public Button RecycleButton { get; private set; }    
    [SerializeField] public TextMeshProUGUI recyclePriceText;



    public void Configure(ObjectDataSO _objectData)
    {
        icon.sprite = _objectData.Icon;
        nameText.text = _objectData.Name;
        recyclePriceText.text = _objectData.RecyclePrice.ToString();

        Color imageColor = ColorHolder.GetColor(_objectData.Rarity);
        nameText.color = imageColor;  

        ConfigureStatContainers(_objectData.BaseStats);
        
        OnSpawned?.Invoke(CollectButton.gameObject);

    }

    private void ConfigureStatContainers(Dictionary<Stat, float> _stats)
    {
        StatContainerManager.GenerateStatContainers(_stats, statContainerParent);
    }
    
}
