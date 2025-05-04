using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;

public class CardOptionUI : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Button lockButton;
    [SerializeField] private Sprite lockedSprite;
    [SerializeField] private Sprite unlockedSprite;
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI costText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private Button selectButton;

    [HideInInspector] public CardSO card;
    private Action onSelected;
    private bool isLocked = false;
    public bool IsLocked => isLocked;

    public void SetCard(CardSO cardData, Action onClick)
    {
        card = cardData;
        onSelected = onClick;

        iconImage.sprite = card.icon;
        nameText.text = card.cardName;
        descriptionText.text = card.description;
        costText.text = $"Cost: {card.cost}";

        selectButton.onClick.RemoveAllListeners();
        selectButton.onClick.AddListener(() => onSelected?.Invoke());
    }

    public void ToggleLockState()
    {
        isLocked = !isLocked;
        SetLockVisual(isLocked);
    }

    private void SetLockVisual(bool locked)
    {
        if (lockButton != null && lockButton.image != null)
        {
            lockButton.image.sprite = locked ? lockedSprite : unlockedSprite;
        }
    }
}