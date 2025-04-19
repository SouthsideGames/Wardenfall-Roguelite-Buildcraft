using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TraitOptionUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private Button selectButton;

    public void Configure(string id, string title, string description, System.Action onClick)
    {
        titleText.text = title;
        descriptionText.text = description;
        selectButton.onClick.RemoveAllListeners();
        selectButton.onClick.AddListener(() => onClick?.Invoke());
    }
}
