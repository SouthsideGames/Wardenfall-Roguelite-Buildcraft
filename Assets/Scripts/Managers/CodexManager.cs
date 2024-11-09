using System;
using UnityEngine;

public class CodexManager : MonoBehaviour
{
    [Header("ELEMENTS:")]
    [SerializeField] private GameObject detailContainer;  
    [SerializeField] private CodexDetailContainerUI detailScript; 
    [SerializeField] private CodexBigCardDetailUI codexBigCardDetailUI;

    private void Awake() 
    {
        HideCodexDetailPanel();
        HideBigCardPanel();
    }

    private void ShowCharacterCodexCategory()
    {
        detailContainer.SetActive(true);
        detailScript.ShowCharacterDetails();
    }

    private void ShowWeaponCodexCategory()
    {
        detailContainer.SetActive(true);
        detailScript.ShowWeaponDetails();
    }

    private void ShowObjectCodexCategory()
    {
        detailContainer.SetActive(true);
        detailScript.ShowObjectDetails();
    }

    private void ShowEnemyCodexCategory()
    {
        detailContainer.SetActive(true);
        detailScript.ShowEnemyDetails();
    }

    public void ShowDetail(Sprite icon, string name, string description)
    {
        codexBigCardDetailUI.gameObject.SetActive(true); 
        codexBigCardDetailUI.Configure(icon, name, description); 
    }

    public void ShowCharacters() => ShowCharacterCodexCategory();
    public void ShowWeapons() => ShowWeaponCodexCategory();
    public void ShowObjects() => ShowObjectCodexCategory();
    public void ShowEnemies() => ShowEnemyCodexCategory();
    public void HideCodexDetailPanel() => detailContainer.SetActive(false); 
    public void HideBigCardPanel() => codexBigCardDetailUI.gameObject.SetActive(false);
}
