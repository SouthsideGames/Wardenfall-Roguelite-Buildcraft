using System.Collections;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;

public class ShopManagerUI : MonoBehaviour
{
    [Header("CHARACTER STATS ELEMENTS:")]
    [SerializeField] private RectTransform characterStatsPanel;
    [SerializeField] private RectTransform characterStatsClosePanel;
    private Vector2 characterStatsEndPos;
    private Vector2 characterStatsStartPos;

    [Header("INVENTORY STATS ELEMENTS:")]
    [SerializeField] private RectTransform inventoryPanel;
    [SerializeField] private RectTransform inventoryClosePanel;
    private Vector2 inventoryEndPos;
    private Vector2 inventoryStartPos;

    [Header("ITEM INFO ELEMENTS:")]
    [SerializeField] private RectTransform itemInfoSlidePanel;
    private Vector2 itemInfoEndPos;
    private Vector2 itemInfoStartPos;

    IEnumerator Start()
    {
        yield return null;

        ConfigureCharacterStatsPanel();
        ConfigureInventoryPanel();
        ConfigureItemInfoPanel();
    }

    private void ConfigureCharacterStatsPanel()
    {
        float width = Screen.width / (4 * characterStatsPanel.lossyScale.x);
        characterStatsPanel.offsetMax = characterStatsPanel.offsetMax.With(x: width);
        characterStatsClosePanel.GetComponent<Image>().raycastTarget = true;

        characterStatsEndPos = characterStatsPanel.anchoredPosition;
        characterStatsStartPos = characterStatsEndPos + Vector2.left * width;

        characterStatsPanel.anchoredPosition = characterStatsStartPos;

        HideCharacterStats();

    }

    public void ShowCharacterStats()
    {
        characterStatsPanel.gameObject.SetActive(true);
        characterStatsClosePanel.gameObject.SetActive(true);
        characterStatsClosePanel.GetComponent<Image>().raycastTarget = true;

        LeanTween.cancel(characterStatsPanel);
        LeanTween.move(characterStatsPanel, characterStatsEndPos, 0.5f).setEase(LeanTweenType.easeInCubic);

        LeanTween.cancel(characterStatsClosePanel);
        LeanTween.alpha(characterStatsClosePanel, 0.8f, 0.5f).setRecursive(false);
    }

    public void HideCharacterStats()
    {
        characterStatsClosePanel.GetComponent<Image>().raycastTarget = false;

        LeanTween.cancel(characterStatsPanel);
        LeanTween.move(characterStatsPanel, characterStatsStartPos, 0.5f)
            .setEase(LeanTweenType.easeOutCubic)
            .setOnComplete(() =>   characterStatsPanel.gameObject.SetActive(false));

        LeanTween.cancel(characterStatsClosePanel);
        LeanTween.alpha(characterStatsClosePanel, 0, 0.5f)
            .setRecursive(false)
            .setOnComplete(() => characterStatsClosePanel.gameObject.SetActive(false));
    }

    private void ConfigureInventoryPanel()
    {
        float width = Screen.width / (4 * inventoryPanel.lossyScale.x);
        inventoryPanel.offsetMin = inventoryPanel.offsetMin.With(x: -width);

        inventoryEndPos = inventoryPanel.anchoredPosition;
        inventoryStartPos = inventoryEndPos - Vector2.left * width;

        inventoryPanel.anchoredPosition = inventoryStartPos;

        HideInventory(false);

    }

    public void ShowInventory()
    {
        inventoryPanel.gameObject.SetActive(true);
        inventoryClosePanel.gameObject.SetActive(true);
        inventoryClosePanel.GetComponent<Image>().raycastTarget = true;

        LeanTween.cancel(inventoryPanel);
        LeanTween.move(inventoryPanel, inventoryEndPos, 0.5f).setEase(LeanTweenType.easeInCubic);

        LeanTween.cancel(inventoryClosePanel);
        LeanTween.alpha(inventoryClosePanel, 0.8f, 0.5f).setRecursive(false);
    }

    public void HideInventory(bool hideItemInfo = true)
    {
        inventoryClosePanel.GetComponent<Image>().raycastTarget = false;

        LeanTween.cancel(inventoryPanel);
        LeanTween.move(inventoryPanel, inventoryStartPos, 0.5f)
            .setEase(LeanTweenType.easeOutCubic)
            .setOnComplete(() =>   inventoryPanel.gameObject.SetActive(false));

        LeanTween.cancel(inventoryClosePanel);
        LeanTween.alpha(inventoryClosePanel, 0, 0.5f)
            .setRecursive(false)
            .setOnComplete(() => inventoryClosePanel.gameObject.SetActive(false));

        if(hideItemInfo)
           HideItemInfoPanel();
    }

    private void ConfigureItemInfoPanel()
    {

        float height = Screen.height / (2 * itemInfoSlidePanel.lossyScale.x);
        itemInfoSlidePanel.offsetMax = itemInfoSlidePanel.offsetMax.With(y: height);

        itemInfoEndPos = itemInfoSlidePanel.anchoredPosition;
        itemInfoStartPos = itemInfoEndPos + Vector2.down * height;

        itemInfoSlidePanel.anchoredPosition = itemInfoStartPos;

        itemInfoSlidePanel.gameObject.SetActive(false);

    }

    public void ShowItemInfoPanel()
    {
        itemInfoSlidePanel.gameObject.SetActive(true);

        itemInfoSlidePanel.LeanCancel();
        itemInfoSlidePanel.LeanMove((Vector3)itemInfoEndPos, .3f)
        .setEase(LeanTweenType.easeOutCubic);
    }

    public void HideItemInfoPanel()
    {
        itemInfoSlidePanel.LeanCancel();
        itemInfoSlidePanel.LeanMove((Vector3)itemInfoStartPos, .3f)
        .setEase(LeanTweenType.easeInCubic)
        .setOnComplete(() => itemInfoSlidePanel.gameObject.SetActive(false));
    }
}
