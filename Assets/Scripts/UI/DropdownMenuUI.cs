using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DropdownMenuUI : MonoBehaviour
{
    [Header("ELEMENTS:")]
    [SerializeField] private Vector2 spacing;

    [Space]
    [Header("SETTINGS:")]
    [SerializeField] private float rotationDuration;
    [SerializeField] private LeanTweenType rotationType;
    
    [Space]
    [Header("ANIMATION:")]
    [SerializeField] private float expandDuration;
    [SerializeField] private float collapseDuration;
    [SerializeField] private LeanTweenType expandType;
    [SerializeField] private LeanTweenType collapseType;

    [Space]
    [Header("FADING:")]
    [SerializeField] private float expandFadeDuration;
    [SerializeField] private float collapseFadeDuration;

    private Button mainButton;
    private DropdownMenuItemUI[] menuItems;
    private bool isExpanded = false;

    private Vector2 buttonPosition;
    private int itemsCount;

    private void Start() 
    {
        itemsCount = transform.childCount - 1;
        menuItems = new DropdownMenuItemUI[itemsCount];
        for(int i = 0; i < itemsCount; i++)
        {
            menuItems[i] = transform.GetChild(i + 1).GetComponent<DropdownMenuItemUI>();
        }    

        mainButton = transform.GetChild(0).GetComponent<Button>();
        mainButton.onClick.AddListener(ToggleMenu);
        mainButton.transform.SetAsLastSibling();   

        buttonPosition = mainButton.transform.position;

        ResetPosition();
    }

    private void ResetPosition()
    {
        for(int i = 0; i < itemsCount; i++)
        {
            menuItems[i].gameObject.SetActive(false);
            menuItems[i].trans.position = buttonPosition;
        }
    }

    private void ToggleMenu()
    {
        isExpanded = !isExpanded;

        if(isExpanded)
        {
            for(int i = 0; i < itemsCount; i++)
            {
                menuItems[i].gameObject.SetActive(true);
<<<<<<<< HEAD:Assets/Scripts/UI/Main Menu/DropdownContainerUI.cs
                menuItems[i].trans.LeanMove(buttonPosition + spacing * (i + 1), expandDuration).setEase(expandType);
          
========
                menuItems[i].trans.position = buttonPosition + spacing * (i + 1);    
>>>>>>>> parent of 61e3b24 (11/10):Assets/Scripts/UI/DropdownMenuUI.cs
            }
        }
        else
        {
            for(int i = 0; i < itemsCount; i++)
            {
                int _index = i;
                menuItems[_index].trans.LeanMove(buttonPosition, collapseDuration)
                    .setEase(collapseType)
                    .setOnComplete(() => menuItems[_index].gameObject.SetActive(false));   
                
            }
        }

        mainButton.transform.LeanRotate(Vector3.forward * 360f, rotationDuration).setEase(rotationType);
    }

    private void OnDestroy() => mainButton.onClick.RemoveAllListeners();
}
