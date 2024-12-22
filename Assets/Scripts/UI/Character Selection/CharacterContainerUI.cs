using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class CharacterContainerUI : MonoBehaviour
{
   [Header("ELEMENTS:")]
    [SerializeField] private Image characterImage;
    [SerializeField] private GameObject backgroundImage;
    [SerializeField] private Image dimImage;
    [SerializeField] private GameObject lockObject;

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
        characterImage.color = Color.grey;
    }

    public void Unlock()
    {
        lockObject.SetActive(false);    
        characterImage.color = Color.white; 

    }
    
}
