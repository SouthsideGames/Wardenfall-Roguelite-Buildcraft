using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;   

public class MenuCardUI : MonoBehaviour
{
    [SerializeField] private Image iconImage;


    public void Configure(Sprite icon) => iconImage.sprite = icon;
}
