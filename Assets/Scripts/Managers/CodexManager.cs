using System;
using UnityEngine;

public class CodexManager : MonoBehaviour
{
    public static Action<GameObject> OnDetailsOpen;
    public static Action<GameObject> OnBigDetailsOpen;
    public static Action<GameObject> OnBigDetailsClosed;

    [Header("ELEMENTS:")]
    [SerializeField] private Transform detailParent;
    [SerializeField] private GameObject detailContainer;  
    [SerializeField] private CodexDetailContainerUI detailScript; 
    [SerializeField] private CodexBigCardDetailUI codexBigCardDetailUI;
    [SerializeField] private Transform codexContainersParent;
    [SerializeField] private GameObject bigCardClosedButton;
    [SerializeField] private GameObject detailBackButton;
    [SerializeField] private GameObject backButton;
  
    [Header("SETTINGS")]
    [SerializeField] private float scrollSpeed;


    private void Awake() 
    {

        InputManager.OnScroll               += ScrollCallback;
        HideCodexDetailPanel();
        HideBigCardPanel();
    }

    private void OnDestroy() => InputManager.OnScroll -= ScrollCallback;

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

        OnBigDetailsOpen?.Invoke(bigCardClosedButton);
    }

    public void ShowCharacters()
    {
        OnDetailsOpen?.Invoke(backButton);
        ShowCharacterCodexCategory();
    }

    public void ShowWeapons()
    {
        OnDetailsOpen?.Invoke(backButton);
        ShowWeaponCodexCategory();
    }

    public void ShowObjects()
    {
        OnDetailsOpen?.Invoke(backButton);
        ShowObjectCodexCategory();
    }

    public void ShowEnemies()
    {
        OnDetailsOpen?.Invoke(backButton);
        ShowEnemyCodexCategory();
    }

    public void HideCodexDetailPanel()
    {
        detailParent.Clear();   
        detailContainer.SetActive(false); 
    }

    public void HideBigCardPanel()
    {
        codexBigCardDetailUI.gameObject.SetActive(false);
        OnBigDetailsClosed?.Invoke(detailBackButton);   
    }

    private void ScrollCallback(float _xValue) => codexContainersParent.GetComponent<RectTransform>().anchoredPosition -= _xValue * scrollSpeed * Time.deltaTime * Vector2.right;

}
