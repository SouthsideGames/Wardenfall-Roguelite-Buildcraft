using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DropdownMenuItemUI : MonoBehaviour
{
    public Image img {get; private set;}    
    public Transform trans {get; private set;}  

    private void Awake() 
    {
        img = GetComponent<Image>();
        trans = GetComponent<Transform>();
    }
}
