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

    [Header("SETTING:")]
    [SerializeField] private float scaleSize = 1.075f;
    [SerializeField] private float animationSpeed = .3f;

    [field: SerializeField] public Button Button { get; private set; }    

    public void Configure(Sprite _icon, string _weaponName, int _level)
    {
        icon.sprite = _icon;
        weaponNameText.text = _weaponName;

        Color imageColor;

        switch(_level)
        {
            case 0:
                imageColor = Color.white;
                break;
            case 1:
                imageColor = Color.blue;
                break;
            case 2:
                imageColor = Color.red;
                break;
            default:
                imageColor = Color.gray;
                break;
        }

        foreach(Image image in containerImages)
        {
           image.color = imageColor;     
        }
          

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
