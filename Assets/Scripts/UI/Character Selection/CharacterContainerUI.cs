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
    [SerializeField] private GameObject roleIcon;
    [SerializeField] private TextMeshProUGUI nameText;

    public Button Button
    {
        get { return GetComponent<Button>(); }
        private set { }
    }

    public void ConfigureCharacterButton(Sprite _characterIcon, string _name, bool _isUnlocked)
    {
        characterImage.sprite = _characterIcon;
        nameText.text = _name;

        if (_isUnlocked)
            Unlock();
        else
            Lock();
    }

    public void Lock()
    {
        lockObject.SetActive(true);

        roleStars.SetActive(false);
        roleIcon.SetActive(false);

        Color newColor = characterImage.color;
        newColor.a = 0.31f;
        characterImage.color = newColor;
    }

    public void Unlock()
    {
        lockObject.SetActive(false);

        roleStars.SetActive(true);
        roleIcon.SetActive(true);

        Color newColor = characterImage.color;
        newColor.a = 1f;
        characterImage.color = newColor;
    }
}
