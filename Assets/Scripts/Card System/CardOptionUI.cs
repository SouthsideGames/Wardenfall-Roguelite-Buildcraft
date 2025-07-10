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

    [Header("Free Use Badge")]
    [SerializeField] private GameObject freeUseBadge;

    [HideInInspector] public CardSO card;
    private Action onSelected;
    private bool isLocked = false;
    public bool IsLocked => isLocked;

    private bool isFreeUse = false;
    public bool IsFreeUse => isFreeUse;

    /// <summary>
    /// Sets up the card option UI.
    /// </summary>
    /// <param name="cardData">Card data for display.</param>
    /// <param name="onClick">Callback for selection.</param>
    /// <param name="locked">If this card is locked between rerolls.</param>
    /// <param name="freeUse">If this card is from the fallback pool.</param>
    public void SetCard(CardSO cardData, Action onClick, bool locked = false, bool freeUse = false)
    {
        card = cardData;
        onSelected = onClick;
        isLocked = locked;
        isFreeUse = freeUse;

        // Set main UI elements
        if (iconImage != null)
            iconImage.sprite = card.icon;

        if (nameText != null)
            nameText.text = card.cardName;

        if (descriptionText != null)
            descriptionText.text = card.description;

        if (costText != null)
            costText.text = $"Cost: {card.cost}";

        // Select button
        if (selectButton != null)
        {
            selectButton.onClick.RemoveAllListeners();
            selectButton.onClick.AddListener(() => onSelected?.Invoke());
        }

        // Lock button
        if (lockButton != null)
        {
            lockButton.onClick.RemoveAllListeners();
            lockButton.onClick.AddListener(ToggleLockState);
            SetLockVisual(isLocked);
        }

        // Show or hide Free Use badge
        if (freeUseBadge != null)
            freeUseBadge.SetActive(isFreeUse);

        // Check if this card would exceed deck cost cap
        bool wouldExceedCap = cardData.cost + CharacterManager.Instance.cards.currentTotalCost
                              > CharacterManager.Instance.cards.GetEffectiveDeckCap();

        if (costText != null)
            costText.color = wouldExceedCap ? Color.red : Color.white;

        if (selectButton != null)
            selectButton.interactable = !wouldExceedCap;
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
