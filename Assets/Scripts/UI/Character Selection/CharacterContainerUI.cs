using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class CharacterContainerUI : MonoBehaviour
{
   [Header("ELEMENTS:")]
    [SerializeField] private Image characterImage;
    [SerializeField] private GameObject lockObject;
    [SerializeField] private GameObject roleStars;
    [SerializeField] private GameObject dimObject;
    [SerializeField] private GameObject roleIcon;

    public Button Button
    {
        get { return GetComponent<Button>(); }

        private set { }
    }

    public void ConfigureCharacterButton(Sprite _characterIcon, bool _isUnlocked)
    {
        characterImage.sprite = _characterIcon;

        if(_isUnlocked)
           Unlock();
        else
            Lock();
    }

    public void Lock()
    {
        lockObject.SetActive(true);
        dimObject.SetActive(true);

        roleStars.SetActive(false);
        roleIcon.SetActive(false);

        Color newColor = characterImage.color; 
        newColor.a = 0.31f;                    
        characterImage.color = newColor;  

    }

    public void Unlock()
    {
        lockObject.SetActive(false);   
        dimObject.SetActive(false); 

        
        roleStars.SetActive(true);
        roleIcon.SetActive(true);

        Color newColor = characterImage.color; 
        newColor.a = 100f;                    
        characterImage.color = newColor;  


    }
    
}
