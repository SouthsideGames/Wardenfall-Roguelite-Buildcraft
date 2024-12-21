using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class CharacterContainerUI : MonoBehaviour
{
    [Header("ELEMENTS:")]
    [SerializeField] private Image characterImage;
    [SerializeField] private TextMeshProUGUI name;
    [SerializeField] private Image roleIcon;
    [SerializeField] private GameObject lockObject;
    [SerializeField] private GameObject selectedObject; 
    public CharacterDataSO characterData; // Reference to character data
    private CharacterInfoPanelUI detailPanel; // Reference to the detail panel
    private bool isUnlocked = false; // Tracks unlock state
    private bool isSelected = false; // Tracks selection state

    public Button Button => GetComponent<Button>();

    public void ConfigureCharacterButton(CharacterDataSO data, bool isUnlocked, CharacterInfoPanelUI panel, bool selected)
    {
        characterData = data;
        detailPanel = panel; 

        characterImage.sprite = data.Icon;
        roleIcon.sprite = data.RoleIcon;
        name.text = data.Name;

        if (isUnlocked)
            Unlock();
        else
            Lock();

    
        SetSelected(selected);

        Button.onClick.RemoveAllListeners();
        Button.onClick.AddListener(OpenDetailPanel); 
    }

    private void OpenDetailPanel()
    {
        if (detailPanel != null)
        {
            detailPanel.ShowPanel(characterData, true); // Activate panel with details
        }
    }

    public void Lock()
    {
        lockObject.SetActive(true);
        characterImage.color = Color.grey;
    }

    public void SetSelected(bool selected)
    {
        isSelected = selected;
        selectedObject.SetActive(selected);
    }

    public void Unlock()
    {
        lockObject.SetActive(false);
        characterImage.color = Color.white;
    }
}
