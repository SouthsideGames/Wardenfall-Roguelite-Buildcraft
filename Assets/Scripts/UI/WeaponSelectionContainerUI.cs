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

    [Header("STATS:")]
    [SerializeField] private Transform statContainerParent;

    [Header("SETTING:")]
    [SerializeField] private float scaleSize = 1.075f;
    [SerializeField] private float animationSpeed = .3f;

    [field: SerializeField] public Button Button { get; private set; }    

    public void Configure(Sprite _icon, string _weaponName, int _level, WeaponDataSO _weaponData)
    {
        icon.sprite = _icon;
        weaponNameText.text = _weaponName;

        Color imageColor = ColorHolder.GetColor(_level);
        weaponNameText.color = imageColor;  

        foreach(Image image in containerImages)
          image.color = imageColor;     

        ConfigureStatContainers(_weaponData);

    }

    private void ConfigureStatContainers(WeaponDataSO _weaponData)
    {
        StatContainerManager.GenerateStatContainers(_weaponData.BaseStats, statContainerParent);
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
