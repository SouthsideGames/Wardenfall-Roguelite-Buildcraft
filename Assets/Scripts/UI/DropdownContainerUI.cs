using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DropdownContainerUI : MonoBehaviour
{
    [Header("ELEMENTS:")]
    [SerializeField] private Vector2 spacing;

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
                menuItems[i].trans.position = buttonPosition + spacing * (i + 1);
          
            }
        }
        else
        {
            for(int i = 0; i < itemsCount; i++)
            {
             
                menuItems[i].trans.position = buttonPosition;
                menuItems[i].gameObject.SetActive(false); 

            }
        }
    }

    private void OnDestroy() => mainButton.onClick.RemoveAllListeners();
}
