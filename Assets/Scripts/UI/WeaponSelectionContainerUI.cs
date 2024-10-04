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

    [Header("SETTING:")]
    [SerializeField] private float scaleSize = 1.075f;
    [SerializeField] private float animationSpeed = .3f;

    [field: SerializeField] public Button Button { get; private set; }    

    public void Configure(Sprite _icon, string _weaponName)
    {
        icon.sprite = _icon;
        weaponNameText.text = _weaponName;

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
