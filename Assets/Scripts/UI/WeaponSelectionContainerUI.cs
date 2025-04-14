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

    public void Configure(WeaponDataSO _weaponData, int _level)
    {
        icon.sprite = _weaponData.Icon;
        weaponNameText.text = _weaponData.Name +  "\n (lvl " + (_level + 1) + ")";

        Color imageColor = ColorHolder.GetColor(_level);
        weaponNameText.color = imageColor;  

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

        LeanTween.scale(gameObject, Vector3.one * scaleSize, animationSpeed).setEase(LeanTweenType.easeOutElastic);

        GameManager.Instance.StartGame();
    }

    public void Deselect()
    {
        LeanTween.cancel(gameObject);
        LeanTween.scale(gameObject, Vector3.one, .3f);
    }

}
