using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class CharacterContainerUI : MonoBehaviour
{
    [Header("ELEMENTS:")]
    [SerializeField] private Image characterImage;
    [SerializeField] private GameObject lockObject;

    public Button Button
    {
        get { return GetComponent<Button>(); }

        private set { }
    }

    public void ConfigureCharacterButton(Sprite _characterIcon)
    {
        characterImage.sprite = _characterIcon;
    }
    
}
