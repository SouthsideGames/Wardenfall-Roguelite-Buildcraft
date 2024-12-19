using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MiniDecklistCardUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI costText;

    private CardSO cardData;
    private LoadoutManager deckManager;
    private Transform originalParent;
    private Vector2 originalPosition;

    public void Configure(Sprite icon, int cost, CardSO cardSO, LoadoutManager manager)
    {
        iconImage.sprite = icon;
        costText.text = cost.ToString();
        cardData = cardSO;
        deckManager = manager;
    }

    public CardSO GetCardData() => cardData;

    public void OnBeginDrag(PointerEventData eventData)
    {
        originalParent = transform.parent;
        originalPosition = GetComponent<RectTransform>().anchoredPosition;
        transform.SetParent(deckManager.transform, true);
    }

    public void OnDrag(PointerEventData eventData) => GetComponent<RectTransform>().anchoredPosition += eventData.delta / deckManager.GetCanvasScaleFactor();
    public void OnEndDrag(PointerEventData eventData) => ResetPosition();
    public void ResetPosition()
    {
        transform.SetParent(originalParent, true);
        GetComponent<RectTransform>().anchoredPosition = originalPosition;
    }

    public bool MatchesCard(CardSO card) => cardData == card;
}
