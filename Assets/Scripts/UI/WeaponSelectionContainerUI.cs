using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class WeaponSelectionContainerUI : MonoBehaviour
{
    [Header("ELEMENTS:")]
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI weaponNameText;

    [Header("LEVEL COLORS:")]
    [SerializeField] private Image[] containerImages;
    [SerializeField] private Outline outline;

    [Header("STATS:")]
    [SerializeField] private Transform statContainerParent;

    [Header("SETTING:")]
    [SerializeField] private float scaleSize = 1.075f;
    [SerializeField] private float animationSpeed = .3f;

    [field: SerializeField] public Button Button { get; private set; }    

    public void Configure(Sprite _icon, string _weaponName, int _level, WeaponDataSO _weaponData)
    {
        icon.sprite = _icon;
        weaponNameText.text = _weaponName +  "\n (lvl " + (_level + 1) + ")";

        Color imageColor = ColorHolder.GetColor(_level);
        weaponNameText.color = imageColor;  

        outline.effectColor = ColorHolder.GetOutlineColor(_level);

        foreach(Image image in containerImages)
          image.color = imageColor;     


        Dictionary<Stat, float> calculatedStats = WeaponStatCalculator.GetStats(_weaponData, _level);
        ConfigureStatContainers(calculatedStats);

    }

    private void ConfigureStatContainers(Dictionary<Stat, float> _calculatedStats)
    {
        StatContainerManager.GenerateStatContainers(_calculatedStats, statContainerParent);
    }

    public void Select()
    {
        LeanTween.cancel(gameObject);

        LeanTween.scale(gameObject, Vector3.one * scaleSize, animationSpeed).setEase(LeanTweenType.easeInOutSine);
    }

    public void Deselect()
    {
        LeanTween.cancel(gameObject);
        LeanTween.scale(gameObject, Vector3.one, .3f);
    }

}
