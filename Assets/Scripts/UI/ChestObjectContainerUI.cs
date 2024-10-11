using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChestObjectContainerUI : MonoBehaviour
{
   [Header("ELEMENTS:")]
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI nameText;

    [Header("STATS:")]
    [SerializeField] private Transform statContainerParent;

    [field: SerializeField] public Button CollectButton { get; private set; }    
    [field: SerializeField] public Button RecycleButton { get; private set; }    

    [Header("LEVEL COLORS:")]
    [SerializeField] private Image[] containerImages;
    [SerializeField] private Outline outline;


    [Header("SETTING:")]
    [SerializeField] private float scaleSize = 1.075f;
    [SerializeField] private float animationSpeed = .3f;

    public void Configure(ObjectDataSO _objectData)
    {
        icon.sprite = _objectData.Icon;
        nameText.text = _objectData.Name;

        Color imageColor = ColorHolder.GetColor(_objectData.Rarity);
        nameText.color = imageColor;  

        outline.effectColor = ColorHolder.GetOutlineColor(_objectData.Rarity);

        foreach(Image image in containerImages)
          image.color = imageColor;     

        ConfigureStatContainers(_objectData.BaseStats);

    }

    private void ConfigureStatContainers(Dictionary<Stat, float> _stats)
    {
        StatContainerManager.GenerateStatContainers(_stats, statContainerParent);
    }
    
}
