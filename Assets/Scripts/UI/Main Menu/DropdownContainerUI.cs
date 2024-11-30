using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DropdownContainerUI : MonoBehaviour
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
    private DropdownContainerItemUI[] menuItems;
    private bool isExpanded = false;

    private Vector2 buttonPosition;
    private int itemsCount;

    private void Start() 
    {
        itemsCount = transform.childCount - 1;
        menuItems = new DropdownContainerItemUI[itemsCount];
        for(int i = 0; i < itemsCount; i++)
        {
            menuItems[i] = transform.GetChild(i + 1).GetComponent<DropdownContainerItemUI>();
        }    

        mainButton = transform.GetChild(0).GetComponent<Button>();
        mainButton.onClick.AddListener(ToggleMenu);
        mainButton.transform.SetAsLastSibling();   

        buttonPosition = mainButton.transform.position;

        ResetPosition();
    }

    private void OnDisable() => ResetPosition();

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
                menuItems[i].trans.LeanMove(buttonPosition + spacing * (i + 1), expandDuration).setEase(expandType);
          
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
