using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CodexMiniCardUI : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI nameText;
    private CodexManager codexManager;
    private string characterName;
    private string characterDescription;
    private Sprite characterIcon;

  
    public void Initialize(Sprite icon, string name, string description, CodexManager manager)
    {
        characterIcon = icon;
        characterName = name;
        characterDescription = description;
        codexManager = manager;

        iconImage.sprite = icon;
        nameText.text = name;

        GetComponent<Button>().onClick.AddListener(OnCardClicked);
    }

    private void OnCardClicked() => codexManager.ShowDetail(characterIcon, characterName, characterDescription);

}
